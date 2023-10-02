namespace RainbowLatinReader;

class CanonLitDoc : ICanonLitDoc {
    private readonly string documentID;
    private readonly Dictionary<string, ICanonLitSection> sections = new();

    public CanonLitDoc(string documentID, Stream latinStream, string latinPath,
        Stream englishStream, string englishPath)
    {
        this.documentID = documentID;

        
    }

    public string GetDocumentID() {
        return documentID;
    }

    public ICanonLitSection? GetSection(string sectionNumber) {
        if (!sections.ContainsKey(sectionNumber)) {
            return null;
        }

        return sections[sectionNumber];
    }
}
