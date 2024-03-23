namespace RainbowLatinReader;

interface ILemmatizedToken {
    public string GetTokenType();
    public string GetLemma();
    public Dictionary<string, string> GetMsd();
}
