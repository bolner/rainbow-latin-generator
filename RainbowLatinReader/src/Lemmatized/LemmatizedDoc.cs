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
using System.Text.RegularExpressions;

namespace RainbowLatinReader;

class LemmatizedDoc : ILemmatizedDoc {
    private readonly ICanonFile file;
    private readonly IXmlParserFactory xmlParserFactory;
    private string title = "";
    private string author = "";
    private string documentID = "";
    private LemmatizedToken[] tokens = [];
    private readonly ILogging logging;
    private Exception? lastError = null;
    private int index = 0;
    
    public LemmatizedDoc(ICanonFile file, IXmlParserFactory xmlParserFactory,
        ILogging logging)
    {
        this.file = file;
        this.xmlParserFactory = xmlParserFactory;
        this.logging = logging;
    }

    public void Process()
    {
        List<LemmatizedToken> list = [];

        try {
            documentID = file.GetDocumentID();
            var parser = xmlParserFactory.GetXmlParser(file, [
                "text.body.ab.w"
            ]);

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

            if (!parser.GoTo("TEI.text.body")) {
                throw new RainbowLatinException("Can't find 'TEI.text.body' in FILE "
                    + $"'{file.GetPath()}'.");
            }

            while (parser.Next()) {
                var attributes = parser.GetAttributes();
                
                if (parser.GetNodeName() != "w") {
                    continue;
                }

                foreach(string field in new string[]{"pos", "msd", "lemma"}) {
                    if (!attributes.ContainsKey(field)) {
                        throw new RainbowLatinException($"Missing property '{field}' on a 'w' element. "
                            + parser.GetDebugInfo());
                    }
                }

                /*
                    Skip punctuation characters and redundant tokens for conjunctions

                    Example for the later:
                    <w rend="unknown" n="4" pos="NOMcom" msd="Case=Acc|Numb=Plur" lemma="seruus">seruosque</w>
                    <w rend="unknown" n="4" pos="CON" msd="MORPH=empty" lemma="que">{seruosque}</w>
                */
                if (attributes["pos"] == "PUNC" || attributes["pos"] == "CON") {
                    continue;
                }

                var value = (parser.FetchContent() ?? "").Trim();
                if (value == "") {
                    throw new RainbowLatinException($"Empty 'w' element. " + parser.GetDebugInfo());
                }

                try {
                    list.Add(
                        new LemmatizedToken(attributes["pos"], value, attributes["msd"], attributes["lemma"])
                    );
                } catch (Exception ex) {
                    throw new RainbowLatinException(ex.Message + " " + parser.GetDebugInfo(), ex);
                }
            }

            tokens = list.ToArray();
            logging.Text("complete", $"{documentID}. Words: {list.Count}");
        } catch (Exception ex) {
            lastError = ex;
            logging.Exception(ex);
        }
    }

    public string GetDocumentID() {
        return documentID;
    }

    public string GetTitle() {
        return title;
    }

    public string GetAuthor() {
        return author;
    }

    public Exception? GetLastError() {
        return lastError;
    }

    public void Rewind() {
        index = 0;
    }

    /// <summary>
    /// Lemmatizes the passed section in a way that keeps
    /// all parts of it.
    /// Call the Rewind() method when starting to process the document.
    /// The LemmatizedDoc object keeps track of the current position
    /// in the lemmatized document. So the Lemmatize() method has to be called
    /// repeatedly in order to progress with the parsing.
    /// </summary>
    /// <param name="section">Latin text.</param>
    /// <returns>List of tokens or null if no match is found.</returns>
    public List<LemmatizedToken>? Lemmatize(string section) {
        if (tokens.Length < 1 || section.Length < 1) {
            return null;
        }

        int baseIndex = index;
        string[] words = Regex.Split(section, @"([^a-zA-ZáÁâæÆàäçċéêèēëËíÍîìïÏñóôœŒòöÖŕúûùüÜýÿ\u0370-\u03ff\u1f00-\u1fff\-]+)");
        int limit = 6;
        List<LemmatizedToken> result = [];

        for(int sectionIndex = 0; sectionIndex < words.Length; sectionIndex++) {
            int end = Math.Min(baseIndex + limit, tokens.Length - 1);
            bool found = false;

            for (int cursor = baseIndex; cursor < end; cursor++) {
                if (words[sectionIndex] == tokens[cursor].GetValue()) {
                    result.Add(tokens[cursor]);
                    baseIndex = cursor + 1;
                    found = true;
                    break;
                }
            }

            if (!found) {
                if (words[sectionIndex] != "") {
                    result.Add(new LemmatizedToken("", words[sectionIndex], "", ""));
                }
            }
        }

        index = baseIndex;
        
        return result;
    }
}
