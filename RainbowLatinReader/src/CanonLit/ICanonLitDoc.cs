namespace RainbowLatinReader;

interface ICanonLitDoc : IProcessable {
    public string GetDocumentID();
    public ICanonLitSection? GetSection(string sectionNumber);

    public string GetEnglishTitle();
    public string GetEnglishAuthor();
    public string GetLatinTitle();
    public string GetLatinAuthor();
}
