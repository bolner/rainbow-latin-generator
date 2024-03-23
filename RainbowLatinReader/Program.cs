using RainbowLatinReader;

var config = new Config(File.Open(Path.Join(Directory.GetCurrentDirectory(),
    "config.ini"), FileMode.Open));
var allowlist = new List<string> { "phi1348.abo011" };
var filter = (string x) => {
    foreach(string allow in allowlist) {
        if (x.Contains(allow)) {
            return true;
        }
    }
    
    return false;
};

/*
    Perseus Canonical Literature
*/
var canonPaths = Directory.EnumerateFiles(
    config.GetPerseusCanonicalLatinLitDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
).Where(filter);

var canonScanner = new DirectoryScanner(canonPaths);
var canonScheduler = new Scheduler<ICanonFile, ICanonLitDoc>(8);
var canonParserFactory = (ICanonFile file, List<string> destinations) => { return new XmlParser(file, destinations); };
var canonLitManager = new CanonLitManager(canonScanner, canonScheduler, canonParserFactory);

/*
    Lemmatized Latin documents
*/
var lemmaPaths = Directory.EnumerateFiles(
    config.GetLatinLemmatizedTextsDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
).Where(filter);

var lemmaScanner = new DirectoryScanner(lemmaPaths);
