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

class CanonLitManager : ICanonLitManager {
    private readonly Dictionary<string, ICanonLitDoc> library = [];

    public CanonLitManager(IDirectoryScanner scanner,
        IScheduler<ICanonLitDoc> scheduler,
        IXmlParserFactory xmlParserFactory)
    {
        ICanonFile? file;
        Dictionary<string, ICanonFile> englishTracker = new();
        Dictionary<string, ICanonFile> latinTracker = new();

        /*
            Find highest versions
        */
        while((file = scanner.Next()) != null) {
            if (file.GetLanguage() == ICanonFile.Language.Latin) {
                if (latinTracker.ContainsKey(file.GetDocumentID())) {
                    if (file.GetVersion() > latinTracker[file.GetDocumentID()].GetVersion()) {
                        latinTracker[file.GetDocumentID()] = file;
                    }
                } else {
                    latinTracker[file.GetDocumentID()] = file;
                }
            } else {
                if (englishTracker.ContainsKey(file.GetDocumentID())) {
                    if (file.GetVersion() > englishTracker[file.GetDocumentID()].GetVersion()) {
                        englishTracker[file.GetDocumentID()] = file;
                    }
                } else {
                    englishTracker[file.GetDocumentID()] = file;
                }
            }
        }

        /*
            Find pairs and schedule them
        */
        foreach(var docID in latinTracker.Keys) {
            if (!englishTracker.ContainsKey(docID)) {
                continue;
            }

            scheduler.AddTask(
                new CanonLitDoc(
                    latinTracker[docID],
                    englishTracker[docID],
                    xmlParserFactory,
                    new BookWorm<string>(),
                    new BookWorm<string>()
                )
            );
        }

        /*
            Parse documents and store results, indexed.
        */
        scheduler.Run();

        foreach(var doc in scheduler.GetResults()) {
            library[doc.GetDocumentID()] = doc;
        }
    }

    public ICanonLitDoc GetDocument(string documentID) {
        ICanonLitDoc? value = null;
        library.TryGetValue(documentID, out value);
        if (value == null) {
            throw new RainbowLatinException($"Cannot find CanonLit document with ID '{documentID}'.");
        }

        return value;
    }
}
