using RainbowLatinReader;

var config = new Config(File.Open(Path.Join(Directory.GetCurrentDirectory(),
    "config.ini"), FileMode.Open));

var paths = Directory.EnumerateFiles(
    config.GetPerseusCanonicalLatinLitDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
);

var scanner = new DirectoryScanner(paths);

