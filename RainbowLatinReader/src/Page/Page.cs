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
    private readonly string outputFilePath;
    private Exception? lastError = null;

    public Page(ICanonLitDoc canonLitDoc, ILemmatizedDoc lemmatizedDoc,
        ITemplateEngine templateEngine, string outputFilePath)
    {
        this.canonLitDoc = canonLitDoc;
        this.lemmatizedDoc = lemmatizedDoc;
        this.templateEngine = templateEngine;
        this.outputFilePath = outputFilePath;
    }

    public void Process() {
        try {
            var keys = canonLitDoc.GetAllSections();
            List<object> sections = [];
            int sectionNumber = 0;

            foreach(string key in keys) {
                sectionNumber++;

                sections.Add(new Dictionary<string, object>() {
                    { "id", key.Replace('=', '-').Replace('|', '_')},
                    { "number", sectionNumber},
                    { "latin", canonLitDoc.GetLatinSection(key) },
                    { "english", canonLitDoc.GetEnglishSection(key) },
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

            var data = new Dictionary<string, object>()
            {
                { "title", canonLitDoc.GetEnglishTitle() },
                { "short_title", shortTitle },
                { "author", canonLitDoc.GetEnglishAuthor() },
                { "short_author", shortAuthor },
                { "sections", sections }
            };

            templateEngine.Generate(data, outputFilePath);
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
