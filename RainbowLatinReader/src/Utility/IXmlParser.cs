namespace RainbowLatinReader;

interface IXmlParser {
    public bool Next(string path);
    public void SetTrap(string path);
    public void ClearTraps();
    public Dictionary<string, List<string>> GetCaptures();
    public Dictionary<string, string> GetProperties();
    public string? GetContent();
}
