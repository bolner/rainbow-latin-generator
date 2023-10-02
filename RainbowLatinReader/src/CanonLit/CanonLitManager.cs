namespace RainbowLatinReader;

class CanonLitManager : ICanonLitManager {
    private readonly IDirectoryScanner scanner;

    public CanonLitManager(string canonLitDir) {
        var paths = Directory.EnumerateFiles(
            canonLitDir,
            "*.perseus-*.xml",
            SearchOption.AllDirectories
        );

        scanner = new DirectoryScanner(paths);
        
    }
}
