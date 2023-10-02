namespace RainbowLatinReader;

class CanonFile : ICanonFile {
    private readonly string path;
    private readonly string documentID;
    private readonly ICanonFile.Language language;
    private readonly int version;
    
    public CanonFile(string path, string documentID,
        ICanonFile.Language language, int version)
    {
        this.path = path;
        this.documentID = documentID;
        this.language = language;
        this.version = version;
    }

    public string GetPath() {
        return path;
    }

    public Stream Open() {
        return File.Open(path, FileMode.Open);
    }

    public string GetDocumentID() {
        return documentID;
    }

    public ICanonFile.Language GetLanguage() {
        return language;
    }

    public int GetVersion() {
        return version;
    }
}
