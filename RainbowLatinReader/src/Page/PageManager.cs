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

class PageManager : IPageManager {
    private readonly ILogging logging;
    private readonly Dictionary<string, IPage> library = [];

    public PageManager(IScheduler<IPage> scheduler, ILogging logging,
        ICanonLitManager canonLitManager, ILemmatizedManager lemmatizedManager,
        IWhitakerManager whitakerManager, ITemplateEngine pageTemplate,
        string outputDirectory)
    {
        this.logging = logging;
        var ids = lemmatizedManager.GetDocumentIDs().ToHashSet();

        foreach(string id in ids) {
            var canonLitDoc = canonLitManager.GetDocument(id);
            var lemmatizedDoc = lemmatizedManager.GetDocument(id);

            scheduler.AddTask(
                new Page(
                    canonLitDoc, lemmatizedDoc, whitakerManager, pageTemplate,
                    outputDirectory
                )
            );
        }

        /*
            Log Perseus documents for which no lemmatized doc exists.
        */
        var canonIDs = canonLitManager.GetDocumentIDs();
        int missingCount = 0;
        foreach(string canonID in canonIDs) {
            if (!ids.Contains(canonID)) {
                missingCount++;
                logging.Warning("missing_lemmatized", canonID);
            }
        }

        if (missingCount > 0) {
            logging.Print($"There are {missingCount} Canonical document pairs for which "
                + "no lemmatized version exists.");
        }
        
        /*
            Generate pages and collect results.
        */
        logging.Print("Generating pages.");
        scheduler.Run();

        foreach(var page in scheduler.GetResults()) {
            if (page.GetLastError() != null) {
                logging.Exception(page.GetLastError() ?? (new RainbowLatinException("Page failed")));
                continue;
            }

            library[page.GetDocumentID()] = page;
            logging.Text("created", page.GetDocumentID());
        }
    }

    public IPage GetPage(string documentID) {
        IPage? value;
        library.TryGetValue(documentID, out value);
        if (value == null) {
            throw new RainbowLatinException($"Cannot find Page with document ID '{documentID}' in page manager.");
        }

        return value;
    }

    public List<string> GetDocumentIDs() {
        return library.Keys.ToList();
    }

    public void GenerateIndexPage(ITemplateEngine indexTemplate, string outputPath) {
        SortedDictionary<string, SortedDictionary<string,
            Dictionary<string, string>>> index = [];

        logging.Print("Generating index page.");

        foreach(var page in library.Values) {
            if (!index.ContainsKey(page.GetEnglishAuthor())) {
                index[page.GetEnglishAuthor()] = [];
            }

            index[page.GetEnglishAuthor()][page.GetEnglishTitle()] = new Dictionary<string, string> {
                { "title", page.GetEnglishTitle() },
                { "link", $"{page.GetDocumentID()}_1.html" },
                { "perseus_code", page.GetDocumentID() },
                { "word_count", page.GetLatinWordCount().ToString() }
            };
        }

        List<Dictionary<string, object>> data = [];

        foreach(var item in index) {
            data.Add(new Dictionary<string, object> {
                { "author", item.Key },
                { "works", item.Value.Values }
            });
        }

        indexTemplate.Generate(
            new Dictionary<string, object>()
            {
                { "index", data },
                { "date", DateTime.Now.ToString("yyyy-MM-dd") },
                { "document_count", library.Count }
            },
            outputPath
        );
    }

    public int GetDocumentCount() {
        return library.Count;
    }
}
