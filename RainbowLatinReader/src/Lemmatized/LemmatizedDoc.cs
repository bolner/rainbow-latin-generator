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
namespace RainbowLatinReader;

class LemmatizedDoc : ILemmatizedDoc {
    private readonly ICanonFile file;
    private readonly IXmlParserFactory xmlParserFactory;
    private string title = "";
    private string author = "";
    private string documentID = "";
    private readonly Dictionary<string, ILemmatizedSection> sectionLookup = [];
    private readonly List<ILemmatizedSection> sections = [];
    private readonly ILogging logging;
    private Exception? lastError = null;
    
    private readonly Dictionary<string, string> tokenTypes = new() {
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

    public LemmatizedDoc(ICanonFile file, IXmlParserFactory xmlParserFactory,
        ILogging logging)
    {
        this.file = file;
        this.xmlParserFactory = xmlParserFactory;
        this.logging = logging;
    }

    public void Process()
    {
        try {
            documentID = file.GetDocumentID();
            var parser = xmlParserFactory.GetXmlParser(file, [
                "text.body.ab",
                "text.body.ab.w"
            ]);

            if (!parser.GoTo("teiHeader.fileDesc.titleStmt.title")) {
                throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.title' in FILE "
                    + $"'{file.GetPath()}'.");
            }
            
            title = (parser.ReadContent() ?? "").Trim();
            if (title == "") {
                throw new RainbowLatinException("Empty 'teiHeader.fileDesc.titleStmt.title' in FILE "
                    + $"'{file.GetPath()}'.");
            }

            parser.GoTo("teiHeader.fileDesc.titleStmt.author");
            author = (parser.ReadContent() ?? "").Trim();
            if (author == "") {
                throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.author' in FILE "
                    + $"'{file.GetPath()}'.");
            }

            if (!parser.GoTo("TEI.text.body")) {
                throw new RainbowLatinException("Can't find 'TEI.text.body' in FILE "
                    + $"'{file.GetPath()}'.");
            }

            LemmatizedSection? section = null;

            while (parser.Next()) {
                // TODO: XmlParser.ReadProperties not working
                var attributes = parser.GetAttributes();

                if (parser.GetNodeName() == "ab") {
                    if (!attributes.ContainsKey("n")) {
                        throw new RainbowLatinException("Missing property 'n' on an 'ab' element. "
                            + parser.GetDebugInfo());
                    }
                    
                    if (section != null) {
                        sections.Add(section);
                        sectionLookup[section.GetSectionNumber()] = section;
                    }

                    section = new LemmatizedSection(attributes["n"]);
                    continue;
                }

                if (section == null) {
                    continue;
                }
                
                if (parser.GetNodeName() != "w") {
                    continue;
                }

                foreach(string field in new string[]{"pos", "msd", "lemma"}) {
                    if (!attributes.ContainsKey(field)) {
                        throw new RainbowLatinException($"Missing property '{field}' on a 'w' element. "
                            + parser.GetDebugInfo());
                    }
                }

                /*
                    Skip punctuation characters and redundant tokens for conjunctions

                    Example for the later:
                    <w rend="unknown" n="4" pos="NOMcom" msd="Case=Acc|Numb=Plur" lemma="seruus">seruosque</w>
                    <w rend="unknown" n="4" pos="CON" msd="MORPH=empty" lemma="que">{seruosque}</w>
                */
                if (attributes["pos"] == "PUNC" || attributes["pos"] == "CON") {
                    continue;
                }

                var value = (parser.ReadContent() ?? "").Trim();
                if (value == "") {
                    throw new RainbowLatinException($"Empty 'w' element. " + parser.GetDebugInfo());
                }

                try {
                    var token = new LemmatizedToken(attributes["pos"], value, attributes["msd"], attributes["lemma"]);

                    /*
                        Validate attributes first
                    */
                    var msd = token.GetMsd();
                    foreach(var pair in msd) {
                        if (!msdKeys.ContainsKey(pair.Key)) {
                            throw new RainbowLatinException($"Unknown 'msd' key: '{pair.Key}'.");
                        }

                        if (!msdValues.ContainsKey(pair.Value)) {
                            throw new RainbowLatinException($"Unknown 'msd' value: '{pair.Value}'.");
                        }
                    }

                    section.AddToken(token);
                } catch (Exception ex) {
                    throw new RainbowLatinException(ex.Message + " " + parser.GetDebugInfo(), ex);
                }
            }

            if (section != null) {
                sections.Add(section);
                sectionLookup[section.GetSectionNumber()] = section;
            }

            logging.Text("complete", $"{documentID}. Sections: {sectionLookup.Count}");
        } catch (Exception ex) {
            lastError = ex;
            logging.Exception(ex);
        }
    }

    public string GetDocumentID() {
        return documentID;
    }

    public ILemmatizedSection? GetSection(string sectionNumber) {
        if (!sectionLookup.ContainsKey(sectionNumber)) {
            return null;
        }

        return sectionLookup[sectionNumber];
    }

    public string GetTitle() {
        return title;
    }

    public string GetAuthor() {
        return author;
    }

    public Exception? GetLastError() {
        return lastError;
    }
}
