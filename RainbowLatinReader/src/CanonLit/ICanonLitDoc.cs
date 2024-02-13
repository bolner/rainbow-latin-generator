namespace RainbowLatinReader;

interface ICanonLitDoc {
    public string GetDocumentID();
    public ICanonLitSection? GetSection(string sectionNumber);

    public string GetEnglishTitle();
    public string GetEnglishAuthor();
    public string GetLatinTitle();
    public string GetLatinAuthor();
}
