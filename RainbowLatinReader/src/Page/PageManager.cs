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
        ITemplateEngine templateEngine)
    {
        this.logging = logging;
        var ids = lemmatizedManager.GetDocumentIDs();

        foreach(string id in ids) {
            var canonLitDoc = canonLitManager.GetDocument(id);
            var lemmatizedDoc = lemmatizedManager.GetDocument(id);

            scheduler.AddTask(
                new Page(
                    canonLitDoc, lemmatizedDoc, templateEngine,
                    Path.Join(Directory.GetCurrentDirectory(), "output")
                )
            );
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
}
