namespace RainbowLatinReader;


interface ICanonFile {
    public enum Language {
        Latin, English
    };

    public string GetPath();
    public Stream Open();
    public string GetDocumentID();
    public Language GetLanguage();
    public int GetVersion();
}
