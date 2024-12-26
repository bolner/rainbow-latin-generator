/*
Copyright 2024 Tamas Bolner

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Text.RegularExpressions;

namespace RainbowLatinReader;

class LemmatizedToken : ILemmatizedToken {
    private readonly string tokenType;
    private readonly string lemma;
    private readonly string value;
    private readonly Dictionary<string, string> msd = [];
    private readonly Regex numericRegex = new(@"\d+$"); // Numbers at the end
    private readonly HashSet<string> notWordTypes = ["", "UNK", "_", "OUT", "PUNC"];

    private readonly Dictionary<string, string> tokenTypes = new() {
        { "", "Unidentified or non-word." },
        { "UNK", "Unknown." },
        { "_", "Unknown." },
        { "ADJadv.mul", "Multiplicative numeral adverbial" },
        { "ADJadv.ord", "Ordinal numeral adverb" },
        { "ADJcar", "Cardinal" },
        { "ADJdis", "Distributive numeral" },
        { "ADJmul", "Multiplicative numeral" },
        { "ADJord", "Ordinal numeral" },
        { "ADJqua", "Adjective" },
        { "ADV", "Adverb" },
        { "ADVint", "Interogative Adverb" },
        { "ADVint.neg", "Negative Interrogative Adverb" },
        { "ADVneg", "Negative Adverb" },
        { "ADVrel", "Relative Adverb" },
        { "CONcoo", "Coordinating conjunction" },
        { "CONsub", "Subordinating conjunction" },
        { "INJ", "Interjection" },
        { "NOMcom", "Noun" },
        { "NOMpro", "Proper Noun" },
        { "OUT", "Out" },
        { "PRE", "Preposition" },
        { "PROdem", "Demonstrative Pronoun" },
        { "PROind", "Indefinite Pronoun" },
        { "PROint", "Interrogative Pronoun" },
        { "PROper", "Personal Pronoun" },
        { "PROpos", "Possessive Pronoun" },
        { "PROpos.ref", "Relfexive Possessive Pronoun" },
        { "PROref", "Reflexive Pronoun" },
        { "PROrel", "Relative Pronoun" },
        { "PUNC", "Punctuation" },
        { "VER", "Verb" },
        { "VERaux", "Auxiliary Verb" },
        { "FOR", "Foreign words" }
    };

    private readonly Dictionary<string, string> msdKeys = new() {
        { "Case", "Case" },
        { "Deg", "Degree" },
        { "Gend", "Gender" },
        { "Mood", "Mood" },
        { "Numb", "Number" },
        { "Person", "Person" },
        { "Tense", "Tense" },
        { "Voice", "Voice" }
    };

    private readonly Dictionary<string, string> msdValues = new() {
        { "1", "1st" },
        { "2", "2nd" },
        { "3", "3rd" },
        { "Act", "Active" },
        { "Acc", "Accusative" },
        { "Adj", "Participle" }, // "Mood=Adj" Participles are verbal adjectives.
        { "Abl", "Ablative" },
        { "Com", "Common" },
        { "Comp", "Comparative" },
        { "Dep", "Deponent" },
        { "Dat", "Dative" },
        { "Imp", "Imperative" },
        { "Impa", "Imperfect" },
        { "Ind", "Indicative" },
        { "Inf", "Infinitive" },
        { "Fem", "Feminine" },
        { "Fut", "Future" },
        { "FutAnt", "Future indicative /OR/ Perfect subjunctive" },
        { "Gen", "Genitive" },
        { "Ger", "Gerund / Participle" }, // Example: "Mood=Ger" for word "dicendi".
        { "Loc", "Locative" },
        { "Masc", "Masculine" },
        { "MascFem", "Masculine / Feminine" },
        { "MascNeut", "Masculine / Neutral" },
        { "Neut", "Neutral" },
        { "Nom", "Nominative" },
        { "Par", "Participle" },
        { "Pass", "Passive" },
        { "Perf", "Perfect" },
        { "PeriFut", "Future participle" },
        { "PeriPerf", "Perfect participle" },
        { "Plur", "Plural" },
        { "Pos", "Positive" },
        { "Pqp", "Pluperfect" },
        { "Pres", "Present" },
        { "SemDep", "Semi-deponent" },
        { "Sing", "Singular" },
        { "Sub", "Subjunctive" },
        { "Sup", "Superlative" },
        { "SupUm", "Supine" }, // Example: "Mood=SupUm" for word "memoratu".
        { "SupU", "Supine" }, // Example: "Mood=SupU" for word "placitum".
        { "Voc", "Vocative" },
    };

    public LemmatizedToken(string tokenType, string value, string msd, string lemma) {
        this.tokenType = tokenType;
        this.value = value;
        this.lemma = numericRegex.Replace(lemma, "");

        var parts = msd.Split('|');
        foreach(var part in parts) {
            if (part.Trim() == "") {
                continue;
            }

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

        /*
            Validate attributes first
        */
        foreach(var pair in this.msd) {
            if (!msdKeys.ContainsKey(pair.Key)) {
                throw new RainbowLatinException($"Unknown 'msd' key: '{pair.Key}'.");
            }

            if (!msdValues.ContainsKey(pair.Value)) {
                throw new RainbowLatinException($"Unknown 'msd' value: '{pair.Value}'.");
            }
        }

        /*
            Validate token type
        */
        if (!tokenTypes.ContainsKey(tokenType)) {
            throw new RainbowLatinException($"Unknown token type: '{tokenType}'.");
        }
    }

    public string GetTokenType() {
        return tokenType;
    }

    public string GetValue() {
        return value;
    }

    public Dictionary<string, string> GetMsd() {
        return new Dictionary<string, string>(msd);
    }

    public string GetTokenTypeReadable() {
        return tokenTypes[tokenType];
    }

    public Dictionary<string, string> GetMsdReadable() {
        Dictionary<string, string> result = [];

        foreach(var item in msd) {
            result[msdKeys[item.Key]] = msdValues[item.Value];
        }

        return result;
    }

    public string GetTemplateClass() {
        if (msd.ContainsKey("Case")) {
            string wordCase = msd["Case"].ToLower();

            if (wordCase == "nom") {
                return "word nom";
            } else if (wordCase == "acc") {
                return "word acc";
            } else if (wordCase == "dat") {
                return "word dat";
            } else if (wordCase == "gen") {
                return "word gen";
            } else if (wordCase == "abl") {
                return "word abl";
            } else if (wordCase == "loc") {
                return "word loc";
            } else if (wordCase == "voc") {
                return "word voc";
            }
        }

        if (msd.ContainsKey("Voice")) {
            string wordVoice = msd["Voice"].ToLower();

            if (wordVoice == "pass") {
                return "word pas";
            } else if (wordVoice == "dep") {
                return "word dep";
            }
        }

        return "word";
    }

    public bool IsWord() {
        return !notWordTypes.Contains(tokenType);
    }
}
