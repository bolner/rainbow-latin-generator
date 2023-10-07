namespace RainbowLatinReader;

class CanonLitManager : ICanonLitManager {
    private readonly Dictionary<string, ICanonLitDoc> library = new();

    public CanonLitManager(DirectoryScanner scanner) {
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
        var scheduler = new Scheduler<ICanonFile, CanonLitDoc>(8);

        foreach(var docID in latinTracker.Keys) {
            if (!englishTracker.ContainsKey(docID)) {
                continue;
            }

            scheduler.AddTask(new SchedulerTask<CanonLitDoc>(
                () => {
                    return new CanonLitDoc(
                        latinTracker[docID],
                        englishTracker[docID]
                    );
                }
            ));
        }

        /*
            Parse documents and store results, indexed.
        */
        scheduler.Run();

        foreach(var doc in scheduler.GetResults()) {
            library[doc.GetDocumentID()] = doc;
        }
    }
}
