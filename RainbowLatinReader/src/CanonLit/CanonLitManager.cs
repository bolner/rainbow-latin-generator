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

class CanonLitManager : ICanonLitManager {
    private readonly ILogging logging;
    private readonly Dictionary<string, ICanonLitDoc> library = [];
    private readonly HashSet<string> documentsRequireLevelClear = [
        "phi0975.phi001", "phi0845.phi002", "stoa0054.stoa006", "phi0448.phi001", "phi0914.phi001",
        "phi1056.phi001", "phi0914.phi0015", "phi0959.phi006", "phi0959.phi001", "phi0474.phi011"
    ];
    private readonly Regex separatorRegex = new(@"[\s\,\.\:\;\(\)\-\!\?—]+",
        RegexOptions.Compiled | RegexOptions.Singleline);
    private readonly Regex latinRegex = new(@"^[a-zA-Z]+$",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public CanonLitManager(IDirectoryScanner scanner,
        IScheduler<ICanonLitDoc> scheduler,
        IXmlParserFactory xmlParserFactory,
        ILogging logging)
    {
        this.logging = logging;
        ICanonFile? file;
        Dictionary<string, ICanonFile> englishTracker = [];
        Dictionary<string, ICanonFile> latinTracker = [];

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

            bool levelClear = documentsRequireLevelClear.Contains(docID);

            scheduler.AddTask(
                new CanonLitDoc(
                    latinTracker[docID],
                    englishTracker[docID],
                    xmlParserFactory,
                    new BookWorm<string>(levelClear),
                    new BookWorm<string>(levelClear),
                    logging
                )
            );
        }

        /*
            Parse documents and store results, indexed.
        */
        logging.Print("Parsing canonical documents.");
        scheduler.Run();

        foreach(var doc in scheduler.GetResults()) {
            if (doc.GetLastError() != null) {
                continue;
            }

            if (doc.IsExcluded()) {
                continue;
            }
            
            library[doc.GetDocumentID()] = doc;
        }
    }

    public ICanonLitDoc GetDocument(string documentID) {
        ICanonLitDoc? value;
        library.TryGetValue(documentID, out value);
        if (value == null) {
            throw new RainbowLatinException($"Cannot find CanonLit document with ID '{documentID}'.");
        }

        return value;
    }

    public List<string> GetDocumentIDs() {
        return library.Keys.ToList();
    }

    public HashSet<string> GetAllWords() {
        HashSet<string> ignoredWords = [];
        HashSet<string> allWords = [];

        logging.Print("Collecting Latin words for the dictionary lookups.");

        foreach(var doc in library.Values) {
            var sectionIDs = doc.GetAllSections();

            foreach(string sectionID in sectionIDs) {
                string section = doc.GetLatinSection(sectionID);
                var words = separatorRegex.Split(section);

                foreach(string word in words) {
                    if (!latinRegex.IsMatch(word)) {
                        if (!ignoredWords.Contains(word)) {
                            logging.Text("skipped_words", word);
                            ignoredWords.Add(word);
                        }

                        continue;
                    }

                    allWords.Add(word);
                }
            }
        }

        return allWords;
    }
}
