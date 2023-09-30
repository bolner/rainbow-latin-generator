namespace RainbowLatinReader;

interface IConfig {
    public string GetLatinLemmatizedTextsDir();
    public string GetPerseusCanonicalLatinLitDir();
    public string GetWhitakerWordsExecutablePath();
    public string GetOutputDir();
    public string GetTemplatesDir();
}
