namespace RainbowLatinReader;

class LemmatizedSection : ILemmatizedSection {
    private readonly string sectionNumber;
    private readonly List<ILemmatizedToken> tokens = [];

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fullSectionString">The full section identifier string.
    /// Example: urn:cts:latinLit:phi1348.abo011.perseus-lat2:2</param>
    public LemmatizedSection(string fullSectionString) {
        sectionNumber = fullSectionString.Split(':').Last();
    }

    public string GetSectionNumber() {
        return sectionNumber;
    }

    public List<ILemmatizedToken> GetTokens() {
        return new List<ILemmatizedToken>(tokens);
    }

    public void AddToken(ILemmatizedToken token) {
        tokens.Add(token);
    }
}
