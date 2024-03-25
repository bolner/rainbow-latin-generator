namespace RainbowLatinReader;

class CanonLitDoc : ICanonLitDoc {
    private readonly ICanonFile latinFile;
    private readonly ICanonFile englishFile;
    private readonly IXmlParserFactory xmlParserFactory;
    private readonly Dictionary<string, ICanonLitSection> sections = new();
    private string latinTitle = "";
    private string englishTitle = "";
    private string latinAuthor = "";
    private string englishAuthor = "";

    public void Process() {
        /*
            Parse English
        */
        Dictionary<string, string> englishSectionLookup;
        ParseDocument(englishFile, xmlParserFactory, out englishTitle, out englishAuthor, out englishSectionLookup);
        
        /*
            Parse Latin
        */
        Dictionary<string, string> latinSectionLookup;
        ParseDocument(latinFile, xmlParserFactory, out latinTitle, out latinAuthor, out latinSectionLookup);

        /*
            Pair sections
        */
        var missing = from x in englishSectionLookup.Keys.Except(latinSectionLookup.Keys) select x;
        if (missing.Any()) {
            throw new Exception($"CanonLitDoc constructor: The English document '{englishFile.GetPath()}' "
                + $"contains section(s) '{String.Join(", ", missing)}' "
                + $"which are not present in the Latin document '{latinFile.GetPath()}'.");
        }

        missing = from x in latinSectionLookup.Keys.Except(englishSectionLookup.Keys) select x;
        if (missing.Any()) {
            throw new Exception($"CanonLitDoc constructor: The Latin document '{latinFile.GetPath()}' "
                + $"contains section(s) '{String.Join(", ", missing)}' "
                + $"which are not present in the English document '{englishFile.GetPath()}'.");
        }
        
        foreach(var key in englishSectionLookup.Keys) {
            sections[key] = new CanonLitSection(
                key,
                latinSectionLookup[key],
                englishSectionLookup[key]
            );
        }
    }

    public CanonLitDoc(ICanonFile latinFile, ICanonFile englishFile,
        IXmlParserFactory xmlParserFactory)
    {
        if (latinFile.GetDocumentID() != englishFile.GetDocumentID()) {
            throw new Exception("CanonLitDoc constructor: The latin and the english "
                + "files have different document IDs.");
        }

        this.latinFile = latinFile;
        this.englishFile = englishFile;
        this.xmlParserFactory = xmlParserFactory;
    }

    public string GetDocumentID() {
        return latinFile.GetDocumentID();
    }

    public ICanonLitSection? GetSection(string sectionNumber) {
        if (!sections.ContainsKey(sectionNumber)) {
            return null;
        }

        return sections[sectionNumber];
    }

    public string GetEnglishTitle() {
        return englishTitle;
    }

    public string GetEnglishAuthor() {
        return englishAuthor;
    }

    public string GetLatinTitle() {
        return latinTitle;
    }

    public string GetLatinAuthor() {
        return latinAuthor;
    }

    private void ParseDocument(ICanonFile file, IXmlParserFactory xmlParserFactory,
        out string title, out string author, out Dictionary<string, string> lookup)
    {
        lookup = new Dictionary<string, string>();

        var parser = xmlParserFactory.GetXmlParser(file, new List<string> {
            "text.body.div",
            "text.body.div1",
            "text.body.milestone"
        });
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

        if (!parser.GoTo("text.body")) {
            throw new RainbowLatinException("Can't find 'TEI.text.body' in FILE "
                + $"'{file.GetPath()}'.");
        }

        var skipSections = new string[]{"note", "intro", "praef"};
        string? prevSection = null;
        bool eof;

        do {
            eof = !parser.Next();
            var attributes = parser.GetAttributes();

            if (!attributes.ContainsKey("n")) {
                continue;
            }

            if (skipSections.Any(attributes["n"].Contains)) {
                continue;
            }

            attributes.TryGetValue("subtype", out string? divSubType);
            attributes.TryGetValue("type", out string? divType);

            if (divSubType != "chapter" && divType != "book") {
                continue;
            }

            if (prevSection == null) {
                prevSection = attributes["n"];
                continue;
            }

            lookup[prevSection] = parser.GetText() ?? "";
            prevSection = attributes["n"];
        } while (!eof);

        if (prevSection != null) {
            lookup[prevSection] = parser.GetText() ?? "";
        }
    }
}
