using RainbowLatinReader;

var config = new Config(File.Open(Path.Join(Directory.GetCurrentDirectory(),
    "config.ini"), FileMode.Open));
var allowlist = new List<string> { "phi1348.abo011" };
var filter = (string x) => {
    return true;
    /*foreach(string allow in allowlist) {
        if (x.Contains(allow)) {
            Console.WriteLine(" - " + x);
            return true;
        }
    }
    
    return false;*/
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
var canonScheduler = new Scheduler<ICanonLitDoc>(1);
var canonParserFactory = new XmlParserFactory();
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
var lemmaScheduler = new Scheduler<ILemmatizedDoc>(1);
var lemmaParserFactory = new XmlParserFactory();
var lemmaManager = new LemmatizedManager(lemmaScanner, lemmaScheduler, lemmaParserFactory);
