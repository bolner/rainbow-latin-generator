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
    private readonly Regex separatorRegex = new(@"[0-9\s\,\.\:\;\(\)\-\!\?—\'\""†\^\>\<]+",
        RegexOptions.Compiled | RegexOptions.Singleline);
    private readonly Regex latinRegex = new(@"^[a-zA-ZáÁâæÆàäçċéêèēëËíÍîìïÏñóôœŒòöÖŕúûùüÜýÿ]+$",
        RegexOptions.Compiled | RegexOptions.Singleline);
    private readonly Regex htmlRegex = new(@"\<\/?[a-z]+(\s+[a-z]+\=\""[^\""]*\"")*\s?\/?\>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public CanonLitManager(ICanonDirectoryScanner scanner,
        IScheduler<ICanonLitDoc> scheduler,
        ICanonLitXmlParserFactory xmlParserFactory,
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

            scheduler.AddTask(
                new CanonLitDoc(
                    latinTracker[docID],
                    englishTracker[docID],
                    xmlParserFactory,
                    new BookWorm<string>(),
                    new BookWorm<string>(),
                    logging
                )
            );
        }

        /*
            Parse documents and store results, indexed.
        */
        logging.Print("Parsing canonical documents.");
        scheduler.Run();
        var results = scheduler.GetResults();

        foreach(var doc in results) {
            if (doc.GetLastError() != null) {
                continue;
            }

            if (doc.IsExcluded()) {
                continue;
            }
            
            library[doc.GetDocumentID()] = doc;
        }

        logging.Print($"Successfully parsed {library.Count * 2} Canonical documents. "
            + $"(Out of {results.Count * 2})");
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
                section = htmlRegex.Replace(section, "");
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
