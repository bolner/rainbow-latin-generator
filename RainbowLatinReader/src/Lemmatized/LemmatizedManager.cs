namespace RainbowLatinReader;

class LemmatizedManager : ILemmatizedManager {
    private readonly Dictionary<string, ILemmatizedDoc> library = new();

    public LemmatizedManager(IDirectoryScanner scanner,
        IScheduler<ILemmatizedDoc> scheduler,
        IXmlParserFactory xmlParserFactory)
    {
        ICanonFile? file;

        /*
            Schedule all parsing tasks
        */
        while((file = scanner.Next()) != null) {
            scheduler.AddTask(new LemmatizedDoc(file, xmlParserFactory));
        }

        /*
            Parse documents and store results, indexed.
        */
        scheduler.Run();

        foreach(var doc in scheduler.GetResults()) {
            library[doc.GetDocumentID()] = doc;
        }
    }

    public ILemmatizedDoc GetDocument(string documentID) {
        ILemmatizedDoc? value = null;
        library.TryGetValue(documentID, out value);
        if (value == null) {
            throw new RainbowLatinException($"Cannot find CanonLit document with ID '{documentID}'.");
        }

        return value;
    }
}
