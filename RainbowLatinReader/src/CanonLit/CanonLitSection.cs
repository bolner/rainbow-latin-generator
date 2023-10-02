namespace RainbowLatinReader;

class CanonLitSection : ICanonLitSection {
    private readonly string sectionNumber;
    private readonly string latinText;
    private readonly string englishText;

    public CanonLitSection(string sectionNumber, string latinText,
        string englishText)
    {
        this.sectionNumber = sectionNumber;
        this.latinText = latinText;
        this.englishText = englishText;
    }

    public string GetSectionNumber() {
        return sectionNumber;
    }

    public string GetLatinText() {
        return latinText;
    }

    public string GetEnglishText() {
        return englishText;
    }
}
