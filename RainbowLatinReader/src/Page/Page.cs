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
using System.Text.Json;

namespace RainbowLatinReader;

class Page : IPage {
    private readonly ICanonLitDoc canonLitDoc;
    private readonly ILemmatizedDoc lemmatizedDoc;
    private readonly IWhitakerManager whitakerManager;
    private readonly ITemplateEngine templateEngine;
    private readonly string outputFolder;
    private Exception? lastError = null;
    private int latinWordCount = 0;
    
    public Page(ICanonLitDoc canonLitDoc, ILemmatizedDoc lemmatizedDoc,
        IWhitakerManager whitakerManager, ITemplateEngine templateEngine,
        string outputFolder)
    {
        this.canonLitDoc = canonLitDoc;
        this.lemmatizedDoc = lemmatizedDoc;
        this.whitakerManager = whitakerManager;
        this.templateEngine = templateEngine;
        this.outputFolder = outputFolder;
    }

    public void Process() {
        try {
            int pageSize = 20;
            var keys = canonLitDoc.GetAllSections().ToArray();
            int count = keys.Length;
            int pageCount = (int)Math.Ceiling(((double)count) / ((double)pageSize));
            int sectionNumber = 0;
            lemmatizedDoc.Rewind();

            /*
                Generate the section key chunks.
            */
            List<string>[] chunks = new List<string>[pageCount];

            for(int page = 0; page < pageCount; page++) {
                int from = page * pageSize;
                int to = Math.Min(from + pageSize - 1, count - 1);

                /*
                    Section key chunks
                */
                List<string> sectionKeys = [];
                for(int i = from; i <= to; i++) {
                    sectionKeys.Add(keys[i]);
                }

                chunks[page] = sectionKeys;
            }

            /*
                Generate each page
            */
            List<WhitakerEntry> whitakerEntries = [];

            for(int page = 0; page < pageCount; page++) {
                whitakerEntries.Clear();
                List<object> sections = [];
                List<string> chunk = chunks[page];

                foreach(string key in chunk) {
                    sectionNumber++;
                    string latinText = canonLitDoc.GetLatinSection(key);
                    var lemmatized = lemmatizedDoc.Lemmatize(latinText);
                    List<Dictionary<string, object>> latinTokens = [];

                    if (lemmatized != null) {
                        latinWordCount += lemmatized.Count;
                        latinTokens = TokenListToTemplateArray(lemmatized, whitakerEntries);
                    }

                    sections.Add(new Dictionary<string, object>() {
                        { "number", sectionNumber},
                        { "latin",  latinTokens},
                        { "english", canonLitDoc.GetEnglishSection(key) }
                    });
                }

                string shortTitle;
                string shortAuthor;

                if (canonLitDoc.GetEnglishTitle().Length > 52) {
                    shortTitle = canonLitDoc.GetEnglishTitle().Left(49) + "...";
                } else {
                    shortTitle = canonLitDoc.GetEnglishTitle();
                }

                if (canonLitDoc.GetEnglishAuthor().Length > 21) {
                    shortAuthor = canonLitDoc.GetEnglishAuthor().Left(18) + "...";
                } else {
                    shortAuthor = canonLitDoc.GetEnglishAuthor();
                }

                /*
                    Data for the navigation bar
                */
                List<object> navigation = [];

                for(int i = 0; i < pageCount; i++) {
                    int from = i * pageSize;
                    int to = Math.Min(from + pageSize - 1, count - 1);

                    navigation.Add(
                        new Dictionary<string, object>() {
                            {"is_current", i == page},
                            {"section_from", from + 1},
                            {"section_to", to + 1},
                            {"page", i + 1}
                        }
                    );
                }

                /*
                    Whitaker entries
                */
                Dictionary<string, string> dict = [];

                foreach(var entry in whitakerEntries) {
                    dict[entry.GetWord()] = entry.GetRawText();
                }

                var data = new Dictionary<string, object>()
                {
                    { "title", canonLitDoc.GetEnglishTitle() },
                    { "short_title", shortTitle },
                    { "author", canonLitDoc.GetEnglishAuthor() },
                    { "short_author", shortAuthor },
                    { "translator", canonLitDoc.GetTranslator() },
                    { "page_count", pageCount},
                    { "current_page", page},
                    { "sections", sections },
                    { "navigation", navigation },
                    { "document_id", canonLitDoc.GetDocumentID() },
                    { "dictionary", JsonSerializer.Serialize(dict) }
                };

                templateEngine.Generate(data, Path.Join(outputFolder, 
                    $"{canonLitDoc.GetDocumentID()}_{page + 1}.html"));
            }
        } catch (Exception ex) {
            lastError = ex;
        }
    }

    public Exception? GetLastError() {
        return lastError;
    }

    public string GetDocumentID() {
        return canonLitDoc.GetDocumentID();
    }

    private List<Dictionary<string, object>> TokenListToTemplateArray(
        List<LemmatizedToken> tokens, List<WhitakerEntry> whitakerEntries
    ) {
        List<Dictionary<string, object>> result = [];
        WhitakerEntry? entry;
        HashSet<string> entryDedup = [];

        foreach(var token in tokens) {
            entry = whitakerManager.GetEntry(token.GetValue());
            if (entry != null) {
                if (!entryDedup.Contains(entry.GetWord())) {
                    whitakerEntries.Add(entry);
                    entryDedup.Add(entry.GetWord());
                }
            }

            result.Add(
                new Dictionary<string, object> {
                    { "class", token.GetTemplateClass() },
                    { "value", token.GetValue() },
                    { "clickable", entry != null },
                    { "is_plain", token.GetTokenType() == "" && entry == null }
                }
            );
        }

        return result;
    }

    public string GetEnglishTitle() {
        return canonLitDoc.GetEnglishTitle();
    }

    public string GetEnglishAuthor() {
        return canonLitDoc.GetEnglishAuthor();
    }

    public string GetTranslator() {
        return canonLitDoc.GetTranslator();
    }

    public int GetLatinWordCount() {
        return latinWordCount;
    }
}
