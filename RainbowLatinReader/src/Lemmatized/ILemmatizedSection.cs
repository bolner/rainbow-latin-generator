namespace RainbowLatinReader;

interface ILemmatizedSection {
    public string GetSectionNumber();
    public List<ILemmatizedToken> GetTokens();
    public void AddToken(ILemmatizedToken token);
}
