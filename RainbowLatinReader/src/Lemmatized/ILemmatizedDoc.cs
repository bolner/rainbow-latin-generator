namespace RainbowLatinReader;

interface ILemmatizedDoc {
    public string GetDocumentID();
    public ILemmatizedSection? GetSection(string sectionNumber);

    public string GetTitle();
    public string GetAuthor();
}
