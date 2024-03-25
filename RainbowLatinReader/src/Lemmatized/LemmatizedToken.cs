using System.Text.RegularExpressions;

namespace RainbowLatinReader;

class LemmatizedToken : ILemmatizedToken {
    private string tokenType;
    private string lemma;
    private string value;
    private Dictionary<string, string> msd = new();
    private Regex numericRegex = new(@"\d+$"); // Numbers at the end

    public LemmatizedToken(string tokenType, string value, string msd, string lemma) {
        this.tokenType = tokenType;
        this.value = value;
        this.lemma = numericRegex.Replace(lemma, "");

        var parts = msd.Split('|');
        foreach(var part in parts) {
            if (part == "_") {
                /*
                    Example:
                    <w rend="chapter" n="1.pr" pos="_" msd="_" lemma="_">Διαιτητικήνsecundam</w>
                */
                continue;
            }

            var keyValue = part.Split('=');
            if (keyValue.Length != 2) {
                throw new RainbowLatinException($"Error while parsing token. Invalid msd: '{msd}'. ");
            }

            this.msd[keyValue[0]] = keyValue[1];
        }

        /*
            Example:
            <w rend="unknown" n="1" pos="CONcoo" msd="MORPH=empty" lemma="sed">sed</w>
        */
        if (this.msd.ContainsKey("MORPH")) {
            if (this.msd["MORPH"] == "empty") {
                this.msd.Remove("MORPH");
            }
        }
    }

    public string GetTokenType() {
        return tokenType;
    }

    public string GetLemma() {
        return lemma;
    }

    public string GetValue() {
        return value;
    }

    public Dictionary<string, string> GetMsd() {
        return new Dictionary<string, string>(msd);
    }
}
