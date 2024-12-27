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
    private readonly ICanonLitXmlParserFactory xmlParserFactory;
    private readonly IBookWorm<string> latinText;
    private readonly IBookWorm<string> englishText;
    private readonly ILogging logging;
    private string latinTitle = "";
    private string englishTitle = "";
    private string latinAuthor = "";
    private string englishAuthor = "";
    private string translator = "";
    private bool isExcluded = false;
    private Exception? lastError = null;
    private bool skipUntilNextSection = false;
    
    private readonly List<string> stops = [
        "text.body.div",
        "text.body.div1",
        "text.body.div2",
        "text.body.div3",
        "text.body.milestone",
        "text.body.seg"
    ];
    private readonly string[] skipSections = ["note", "intro", "chronology"];
    private readonly string[] skipText = ["", "* * *"];
    private readonly string[] skipTypes = ["", "translation", "edition"];

    public CanonLitDoc(ICanonFile latinFile, ICanonFile englishFile,
        ICanonLitXmlParserFactory xmlParserFactory, IBookWorm<string> latinText,
        IBookWorm<string> englishText, ILogging logging)
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
        this.logging = logging;
    }

    public void Process() {
        /*
            Parse English and Latin
        */

        try {
            /*
                First pass
                (Find all section types and select the ones
                that are present in both the English and Latin document.)
            */
            var engSecTypes = FindSectionTypes(englishFile);
            var latSecTypes = FindSectionTypes(latinFile);

            HashSet<string> common = [.. engSecTypes.Intersect(latSecTypes) ?? []];

            /*
                Check for exclusion
            */
            if (common.Count == 0) {
                isExcluded = true;
                logging.Warning("mismatch", $"Document '{englishFile.GetPath()}' is excluded because the English "
                    + "and the Latin versions have no matching sections.");
                return;
            }

            if (common.Count == 1 && common.Contains("book")) {
                isExcluded = true;
                logging.Warning("mismatch", $"Document '{englishFile.GetPath()}' is excluded because only the book "
                    + "level is common between the English and the Latin versions.");
                return;
            }

            /*
                Second pass
                (Partition the text by the selected section types.)
            */
            string latinEditor;

            ParseDocument(englishFile, common, out englishTitle, out englishAuthor,
                out translator, englishText);
            ParseDocument(latinFile, common, out latinTitle, out latinAuthor,
                out latinEditor, latinText);

            /*
                Pair sections
                - Display warning
                - Keep only the common ones
            */
            var englishSections = englishText.GetSectionKeyList();
            var latinSections = latinText.GetSectionKeyList();

            var missing = from x in englishSections.Except(latinSections) select x;
            if (missing.Any()) {
                isExcluded = true;
                logging.Warning("mismatch", $"CanonLitDoc constructor: The English document '{englishFile.GetPath()}' "
                    + $"contains section(s) '{string.Join(", ", missing)}' "
                    + $"which are not present in the Latin document '{latinFile.GetPath()}'. "
                    + $"First text: {englishText.GetFirstNodeBySectionKey(missing.First() ?? "")?.Value}");
                
                return;
            }

            missing = from x in latinSections.Except(englishSections) select x;
            if (missing.Any()) {
                isExcluded = true;
                logging.Warning("mismatch", $"CanonLitDoc constructor: The Latin document '{latinFile.GetPath()}' "
                    + $"contains section(s) '{string.Join(", ", missing)}' "
                    + $"which are not present in the English document '{englishFile.GetPath()}'. "
                    + $"First text: {latinText.GetFirstNodeBySectionKey(missing.First() ?? "")?.Value}");
                
                return;
            }

            if (englishSections.Count < 10) {
                isExcluded = true;
                logging.Warning("mismatch", $"Document '{englishFile.GetPath()}' is excluded because of the "
                    + $"low number of sections: {englishSections.Count}");
                
                return;
            }

            logging.Text("complete", $"{latinFile.GetDocumentID()}. "
                + $"English sections: {englishSections.Count}. Latin sections: {latinSections.Count}. "
                + string.Join(", ", latinSections));
        } catch (Exception ex) {
            lastError = ex;
            logging.Exception(ex);
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

    public List<string> GetAllSections() {
        return latinText.GetSectionKeyList();
    }

    private HashSet<string> FindSectionTypes(ICanonFile file) {
        HashSet<string> result = [];
        bool eof;

        using var parser = xmlParserFactory.GetXmlParser(file, stops);
        
        do {
            eof = !parser.Next();
            
            try {
                ParseForSection(parser, null, out string? sectionType, out string? sectionName);
                if (sectionType != null) {
                    result.Add(sectionType);
                }
            } catch (Exception ex) {
                throw new RainbowLatinException($"FILE '{file.GetPath()}': {ex.Message}", ex);
            }
        } while (!eof);

        return result;
    }

    private void ParseDocument(ICanonFile file, HashSet<string> allowedSectionTypes,
        out string title, out string author, out string editor, IBookWorm<string> bookworm)
    {
        bool eof;
        using var parser = xmlParserFactory.GetXmlParser(file, stops);

        if (!parser.GoTo("teiHeader.fileDesc.titleStmt.title")) {
            throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.title' in FILE "
                + $"'{file.GetPath()}'.");
        }

        title = (parser.FetchContent() ?? "").Trim();
        if (title == "") {
            throw new RainbowLatinException("Empty 'teiHeader.fileDesc.titleStmt.title' in FILE "
                + $"'{file.GetPath()}'.");
        }

        parser.GoTo("teiHeader.fileDesc.titleStmt.author");
        author = (parser.FetchContent() ?? "").Trim();
        if (author == "") {
            throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.author' in FILE "
                + $"'{file.GetPath()}'.");
        }

        parser.GoTo("teiHeader.fileDesc.titleStmt.editor", "text.body");
        editor = (parser.FetchContent() ?? "").Trim();
        if (editor == "") {
            logging.Warning("missing", "Missing 'teiHeader.fileDesc.titleStmt.editor' in FILE "
                + $"'{file.GetPath()}'.");
        }

        if (!parser.GoTo("text.body")) {
            throw new RainbowLatinException("Can't find 'TEI.text.body' in FILE "
                + $"'{file.GetPath()}'.");
        }

        do {
            eof = !parser.Next();

            var text = parser.FetchTextBuffer() ?? "";
            if (!skipText.Any(text.Trim().Equals) && !skipUntilNextSection) {
                bookworm.AddElement(text);
            }

            ParseForSection(parser, allowedSectionTypes, out string? sectionType, out string? sectionName);

            if (sectionType != null) {
                if (sectionName == null || sectionName == "") {
                    logging.Warning("syntax", "Encountered a milestone that has only 'section type', but no 'section name'. "
                        + $"Location: {parser.GetDebugInfo()}");
                }
                
                try {
                    bookworm.IncomingSection(sectionType, sectionName ?? "");
                    skipUntilNextSection = false;
                } catch (Exception ex) {
                    throw new RainbowLatinException($"{parser.GetDebugInfo()}: {ex.Message}", ex);
                }
            }
        } while (!eof);
    }

    private void ParseForSection(ICanonLitXmlParser parser, HashSet<string>? allowedSectionTypes,
        out string? sectionType, out string? sectionName)
    {
        sectionType = null;
        sectionName = null;
        var attributes = parser.GetAttributes();
        attributes.TryGetValue("type", out string? divType);

        // Example: phi0917.phi001.perseus-lat2.xml
        if (divType == "commentary") {
            parser.Skip();
            return;
        }

        attributes.TryGetValue("n", out sectionName);
        if (skipSections.Any((sectionName ?? "").Equals)) {
            sectionName = null;
            skipUntilNextSection = true;
            return;
        }

        if (sectionType == null) {
            attributes.TryGetValue("subtype", out string? divSubType);
            attributes.TryGetValue("unit", out string? unit);

            if (divSubType != null) {
                sectionType = divSubType;
            } else if (unit != null) {
                sectionType = unit;
            } else {
                sectionType = divType;
            }
        }

        if ((sectionType == "chapter" || sectionType == "poem") && (sectionName == "pr" || sectionName == "0")) {
            sectionName = "praef";
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

    public string GetEnglishSection(string sectionKey) {
        List<string> parts = [];
        var cursor = englishText.GetFirstNodeBySectionKey(sectionKey);
        var last = englishText.GetLastNodeBySectionKey(sectionKey);

        if (cursor == null) {
            throw new RainbowLatinException($"Cannot find section '{sectionKey}' in file '{englishFile.GetPath()}'.");
        }

        if (last == null) {
            throw new RainbowLatinException($"Cannot find section ending for '{sectionKey}' in file '{englishFile.GetPath()}'.");
        }

        do {
            parts.Add(cursor.Value.Trim());

            if (cursor == last) {
                break;
            }

            cursor = cursor.Next;
        } while(cursor != null);

        return string.Join(' ', parts).Replace("()", "");
    }

    public string GetLatinSection(string sectionKey) {
        List<string> parts = [];
        var cursor = latinText.GetFirstNodeBySectionKey(sectionKey);
        var last = latinText.GetLastNodeBySectionKey(sectionKey);

        if (cursor == null) {
            throw new RainbowLatinException($"Cannot find section '{sectionKey}' in file '{latinFile.GetPath()}'.");
        }

        if (last == null) {
            throw new RainbowLatinException($"Cannot find section ending for '{sectionKey}' in file '{latinFile.GetPath()}'.");
        }

        do {
            parts.Add(cursor.Value.Trim());

            if (cursor == last) {
                break;
            }

            cursor = cursor.Next;
        } while(cursor != null);

        return string.Join(' ', parts).Replace("()", "");
    }

    public bool IsExcluded() {
        return isExcluded;
    }

    public Exception? GetLastError() {
        return lastError;
    }

    public string GetTranslator() {
        return translator;
    }
}
