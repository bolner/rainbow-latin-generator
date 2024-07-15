/*
Copyright 2024 Tamas Bolner

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
namespace RainbowLatinReader;

class CanonLitDoc : ICanonLitDoc {
    private readonly ICanonFile latinFile;
    private readonly ICanonFile englishFile;
    private readonly IXmlParserFactory xmlParserFactory;
    private readonly IBookWorm<string> latinText;
    private readonly IBookWorm<string> englishText;
    private string latinTitle = "";
    private string englishTitle = "";
    private string latinAuthor = "";
    private string englishAuthor = "";

    private readonly string[] skipSections = ["note", "intro", "praef"];
    private readonly string[] skipText = ["", "* * *"];
    private readonly string[] skipTypes = ["", "translation", "edition", "section"];
    private int pbIndex = 1;

    public CanonLitDoc(ICanonFile latinFile, ICanonFile englishFile,
        IXmlParserFactory xmlParserFactory, IBookWorm<string> latinText,
        IBookWorm<string> englishText)
    {
        if (latinFile.GetDocumentID() != englishFile.GetDocumentID()) {
            throw new Exception("CanonLitDoc constructor: The latin and the english "
                + "files have different document IDs.");
        }

        this.latinFile = latinFile;
        this.englishFile = englishFile;
        this.xmlParserFactory = xmlParserFactory;
        this.latinText = latinText;
        this.englishText = englishText;
    }

    public void Process() {
        /*
            Parse English and Latin
        */

        /*
            First pass
            (Find all section types and select the ones
            that are present in both the English and Latin document.)
        */
        pbIndex = 1;
        var engSecTypes = FindSectionTypes(englishFile);

        pbIndex = 1;
        var latSecTypes = FindSectionTypes(latinFile);

        HashSet<string> common = new(engSecTypes.Intersect(latSecTypes) ?? []);

        /*
            Second pass
            (Partition the text by the selected section types.)
        */
        pbIndex = 1;
        ParseDocument(englishFile, common, out englishTitle, out englishAuthor, englishText);

        pbIndex = 1;
        ParseDocument(latinFile, common, out latinTitle, out latinAuthor, latinText);

        /*
            Add missing
        */
        // TODO

        /*
            Pair sections
        */
        var englishSections = englishText.GetSectionKeyList();
        var latinSections = latinText.GetSectionKeyList();

        Console.WriteLine(englishText.GetFirstNodeBySectionKey("chapter=49")?.Value);

        var missing = from x in englishSections.Except(latinSections) select x;
        if (missing.Any()) {
            throw new RainbowLatinException($"CanonLitDoc constructor: The English document '{englishFile.GetPath()}' "
                + $"contains section(s) '{String.Join(", ", missing)}' "
                + $"which are not present in the Latin document '{latinFile.GetPath()}'. "
                + $"First text: {englishText.GetFirstNodeBySectionKey(missing.First() ?? "")?.Value}");
        }

        missing = from x in latinSections.Except(englishSections) select x;
        if (missing.Any()) {
            throw new RainbowLatinException($"CanonLitDoc constructor: The Latin document '{latinFile.GetPath()}' "
                + $"contains section(s) '{String.Join(", ", missing)}' "
                + $"which are not present in the English document '{englishFile.GetPath()}'. "
                + $"First text: {latinText.GetFirstNodeBySectionKey(missing.First() ?? "")?.Value}");
        }
    }

    public string GetDocumentID() {
        return latinFile.GetDocumentID();
    }

    public string GetEnglishTitle() {
        return englishTitle;
    }

    public string GetEnglishAuthor() {
        return englishAuthor;
    }

    public string GetLatinTitle() {
        return latinTitle;
    }

    public string GetLatinAuthor() {
        return latinAuthor;
    }

    private HashSet<string> FindSectionTypes(ICanonFile file) {
        HashSet<string> result = [];
        bool eof;

        using var parser = xmlParserFactory.GetXmlParser(file, [
            "text.body.div",
            "text.body.div1",
            "text.body.div2",
            "text.body.pb",
            "text.body.milestone"
        ]);
        
        do {
            eof = !parser.Next();
            
            ParseForSection(parser, null, out string? sectionType, out string? sectionName);
            if (sectionType != null) {
                result.Add(sectionType);
            }
        } while (!eof);

        return result;
    }

    private void ParseDocument(ICanonFile file, HashSet<string> allowedSectionTypes,
        out string title, out string author, IBookWorm<string> bookworm)
    {
        bool eof;
        using var parser = xmlParserFactory.GetXmlParser(file, [
            "text.body.div",
            "text.body.div1",
            "text.body.div2",
            "text.body.pb",
            "text.body.milestone"
        ]);

        if (!parser.GoTo("teiHeader.fileDesc.titleStmt.title")) {
            throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.title' in FILE "
                + $"'{file.GetPath()}'.");
        }

        title = (parser.ReadContent() ?? "").Trim();
        if (title == "") {
            throw new RainbowLatinException("Empty 'teiHeader.fileDesc.titleStmt.title' in FILE "
                + $"'{file.GetPath()}'.");
        }

        parser.GoTo("teiHeader.fileDesc.titleStmt.author");
        author = (parser.ReadContent() ?? "").Trim();
        if (author == "") {
            throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.author' in FILE "
                + $"'{file.GetPath()}'.");
        }

        if (!parser.GoTo("text.body")) {
            throw new RainbowLatinException("Can't find 'TEI.text.body' in FILE "
                + $"'{file.GetPath()}'.");
        }

        do {
            eof = !parser.Next();
            
            var text = parser.GetText() ?? "";
            if (!skipText.Any(text.Trim().Equals)) {
                bookworm.AddElement(text);
            }

            ParseForSection(parser, allowedSectionTypes, out string? sectionType, out string? sectionName);
            if (sectionType != null && sectionName != null) {
                bookworm.IncomingSection(sectionType, sectionName);
            }
        } while (!eof);
    }

    private void ParseForSection(IXmlParser parser, HashSet<string>? allowedSectionTypes,
        out string? sectionType, out string? sectionName)
    {
        sectionType = null;
        sectionName = null;
        var attributes = parser.GetAttributes();

        attributes.TryGetValue("n", out sectionName);
        if (skipSections.Any((sectionName ?? "").Equals)) {
            sectionName = null;
            return;
        }

        if (parser.GetNodeName() == "pb") {
            sectionName = pbIndex.ToString();
            pbIndex++;
            sectionType = "pb";
        }

        if (sectionType == null) {
            attributes.TryGetValue("subtype", out string? divSubType);
            attributes.TryGetValue("type", out string? divType);

            if (divSubType != null) {
                sectionType = divSubType;
            } else {
                sectionType = divType;
            }
        }

        // Skip these: <div2 type="chapter" n="2">
        if (parser.GetNodeName() == "div2" && sectionType == "chapter") {
            sectionType = null;
            sectionName = null;
            return;
        }
        
        if (sectionType != null) {
            if (allowedSectionTypes != null) {
                if (!allowedSectionTypes.Contains(sectionType)) {
                    sectionType = null;
                    return;
                }
            }
            
            if (skipTypes.Any((sectionType ?? "").Equals)) {
                sectionType = null;
            }
        }
    }

    public List<string> GetEnglishSection(string sectionKey) {
        List<string> paragraphs = [];
        var cursor = englishText.GetFirstNodeBySectionKey(sectionKey);

        if (cursor == null) {
            throw new RainbowLatinException($"Cannot find section '{sectionKey}' in file '{englishFile.GetPath()}'.");
        }

        do {
            paragraphs.Add(cursor.Value);
            cursor = cursor.Next;
        } while(cursor != null);

        return paragraphs;
    }

    public List<string> GetLatinSection(string sectionKey) {
        List<string> paragraphs = [];
        var cursor = latinText.GetFirstNodeBySectionKey(sectionKey);

        if (cursor == null) {
            throw new RainbowLatinException($"Cannot find section '{sectionKey}' in file '{latinFile.GetPath()}'.");
        }

        do {
            paragraphs.Add(cursor.Value);
            cursor = cursor.Next;
        } while(cursor != null);

        return paragraphs;
    }
}
