namespace RainbowLatinReader;

interface IXmlParser {
    public bool GoTo(string destination);
    public bool Next();
    public Dictionary<string, string> GetAttributes();
    public string? GetText();
    public string? ReadContent();
}
