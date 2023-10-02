namespace RainbowLatinReader;

interface ICanonLitDoc {
    public string GetDocumentID();
    public ICanonLitSection? GetSection(string sectionNumber);
}
