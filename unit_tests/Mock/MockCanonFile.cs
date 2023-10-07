using RainbowLatinReader;

namespace unit_tests;


sealed class MockCanonFile : ICanonFile {
    private readonly string path;
    private readonly string documentID;
    private readonly ICanonFile.Language language;
    private readonly int version;
    private readonly byte[] content;

    public MockCanonFile(string path, string documentID, ICanonFile.Language language,
        int version, byte[] content)
    {
        this.path = path;
        this.documentID = documentID;
        this.language = language;
        this.version = version;
        this.content = content;
    }

    public string GetPath() {
        return path;
    }

    public Stream Open() {
        return new MemoryStream(content);
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