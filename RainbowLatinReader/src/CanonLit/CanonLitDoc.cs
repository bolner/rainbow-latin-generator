namespace RainbowLatinReader;

class CanonLitDoc : ICanonLitDoc {
    private readonly ICanonFile latinFile;
    private readonly ICanonFile englishFile;
    private readonly Dictionary<string, ICanonLitSection> sections = new();

    public CanonLitDoc(ICanonFile latinFile, ICanonFile englishFile)
    {
        if (latinFile.GetDocumentID() != englishFile.GetDocumentID()) {
            throw new Exception("CanonLitDoc constructor: The latin and the english "
                + "files have different document IDs.");
        }

        this.latinFile = latinFile;
        this.englishFile = englishFile;
    }

    public string GetDocumentID() {
        return latinFile.GetDocumentID();
    }

    public ICanonLitSection? GetSection(string sectionNumber) {
        if (!sections.ContainsKey(sectionNumber)) {
            return null;
        }

        return sections[sectionNumber];
    }
}
