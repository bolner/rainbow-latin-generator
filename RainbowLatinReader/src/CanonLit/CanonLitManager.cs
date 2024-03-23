namespace RainbowLatinReader;

class CanonLitManager : ICanonLitManager {
    private readonly Dictionary<string, ICanonLitDoc> library = new();

    public CanonLitManager(IDirectoryScanner scanner,
        IScheduler<ICanonFile, ICanonLitDoc> scheduler,
        Func<ICanonFile, List<string>, IXmlParser> xmlParserFactory)
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

            scheduler.AddTask(new SchedulerTask<ICanonLitDoc>(
                () => {
                    return new CanonLitDoc(
                        latinTracker[docID],
                        englishTracker[docID],
                        xmlParserFactory
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

    public ICanonLitDoc GetDocument(string documentID) {
        ICanonLitDoc? value = null;
        library.TryGetValue(documentID, out value);
        if (value == null) {
            throw new RainbowLatinException($"Cannot find CanonLit document with ID '{documentID}'.");
        }

        return value;
    }
}
