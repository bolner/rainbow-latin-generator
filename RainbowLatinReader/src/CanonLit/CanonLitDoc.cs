namespace RainbowLatinReader;

class CanonLitDoc : ICanonLitDoc {
    private readonly ICanonFile latinFile;
    private readonly ICanonFile englishFile;
    private readonly Dictionary<string, ICanonLitSection> sections = new();
    private readonly string title;
    private readonly string author;

    public CanonLitDoc(ICanonFile latinFile, ICanonFile englishFile,
        Func<ICanonFile, List<string>, IXmlParser> xmlParserFactory)
    {
        if (latinFile.GetDocumentID() != englishFile.GetDocumentID()) {
            throw new Exception("CanonLitDoc constructor: The latin and the english "
                + "files have different document IDs.");
        }

        this.latinFile = latinFile;
        this.englishFile = englishFile;

        /*
            Parse English
        */
        var parser = xmlParserFactory(englishFile, new List<string> {
            "text.body.div",
            "text.body.milestone"
        });
        if (!parser.GoTo("teiHeader.fileDesc.titleStmt.title")) {
            throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.title' in FILE "
                + $"'{englishFile.GetPath()}'.");
        }
        
        title = (parser.ReadContent() ?? "").Trim();
        if (title == "") {
            throw new RainbowLatinException("Empty 'teiHeader.fileDesc.titleStmt.title' in FILE "
                + $"'{englishFile.GetPath()}'.");
        }

        parser.GoTo("teiHeader.fileDesc.titleStmt.author");
        author = (parser.ReadContent() ?? "").Trim();
        if (author == "") {
            throw new RainbowLatinException("Missing 'teiHeader.fileDesc.titleStmt.author' in FILE "
                + $"'{englishFile.GetPath()}'.");
        }

        
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
}
