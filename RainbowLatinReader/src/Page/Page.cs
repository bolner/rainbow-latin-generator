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

class Page : IPage {
    private readonly ICanonLitDoc canonLitDoc;
    private readonly ILemmatizedDoc lemmatizedDoc;
    private readonly ITemplateEngine templateEngine;
    private readonly string outputFolder;
    private Exception? lastError = null;

    public Page(ICanonLitDoc canonLitDoc, ILemmatizedDoc lemmatizedDoc,
        ITemplateEngine templateEngine, string outputFolder)
    {
        this.canonLitDoc = canonLitDoc;
        this.lemmatizedDoc = lemmatizedDoc;
        this.templateEngine = templateEngine;
        this.outputFolder = outputFolder;
    }

    public void Process() {
        try {
            int pageSize = 30;
            var keys = canonLitDoc.GetAllSections().ToArray();
            int count = keys.Length;
            int pageCount = (int)Math.Ceiling(((double)count) / ((double)pageSize));
            int sectionNumber = 0;

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
            for(int page = 0; page < pageCount; page++) {
                List<object> sections = [];
                List<string> chunk = chunks[page];

                foreach(string key in chunk) {
                    sectionNumber++;

                    sections.Add(new Dictionary<string, object>() {
                        { "number", sectionNumber},
                        { "latin", canonLitDoc.GetLatinSection(key) },
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

                var data = new Dictionary<string, object>()
                {
                    { "title", canonLitDoc.GetEnglishTitle() },
                    { "short_title", shortTitle },
                    { "author", canonLitDoc.GetEnglishAuthor() },
                    { "short_author", shortAuthor },
                    { "page_count", pageCount},
                    { "current_page", page},
                    { "sections", sections },
                    { "navigation", navigation },
                    { "document_id", canonLitDoc.GetDocumentID() }
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
}
