namespace RainbowLatinReader;

using System.IO;
using Microsoft.Extensions.Configuration;

class Config : IConfig {
    private readonly string latinLemmatizedTextsDir;
    private readonly string perseusCanonicalLatinLitDir;
    private readonly string whitakerWordsExecutablePath;
    private readonly string outputDir;
    private readonly string templatesDir;

    public Config(Stream stream) {
        IConfiguration config;

        try {
            config = new ConfigurationBuilder()
                .AddIniStream(stream)
                .Build();
        } catch (Exception ex) {
            throw new RainbowLatinException($"Cannot open or parse the config file 'config.ini'. "
                + "Please make sure that the work directory contains the file.", ex);
        }

    	IConfigurationSection section = config.GetSection("Paths");
        if (!section.Exists()) {
            throw new RainbowLatinException($"Cannot find section '[File Paths]' in config file 'config.ini'.");
        }

        latinLemmatizedTextsDir = (section["latin_lemmatized_texts.dir"] ?? "").Trim();
        perseusCanonicalLatinLitDir = (section["perseus_canonical_latinLit.dir"] ?? "").Trim();
        whitakerWordsExecutablePath = (section["whitaker_words_executable.path"] ?? "").Trim();
        outputDir = (section["output.dir"] ?? "").Trim();
        templatesDir = (section["templates.dir"] ?? "").Trim();

        /*
            Validate
        */
        if (latinLemmatizedTextsDir == "" || !Directory.Exists(latinLemmatizedTextsDir)) {
            throw new RainbowLatinException($"The 'latin_lemmatized_texts.dir' setting in config file 'config.ini' "
                + $"does not contain a valid directory path. Value: '{latinLemmatizedTextsDir}'.");
        }

        if (perseusCanonicalLatinLitDir == "" || !Directory.Exists(perseusCanonicalLatinLitDir)) {
            throw new RainbowLatinException($"The 'perseus_canonical_latinLit.dir' setting in config file 'config.ini' "
                + $"does not contain a valid directory path. Value: '{perseusCanonicalLatinLitDir}'.");
        }

        if (whitakerWordsExecutablePath == "" || !File.Exists(whitakerWordsExecutablePath)) {
            throw new RainbowLatinException($"The 'whitaker_words_executable.path' setting in config file 'config.ini' "
                + $"does not contain a valid file path. Value: '{whitakerWordsExecutablePath}'.");
        }

        if (outputDir == "" || !Directory.Exists(outputDir)) {
            throw new RainbowLatinException($"The 'output.dir' setting in config file 'config.ini' "
                + $"does not contain a valid directory path. Value: '{outputDir}'.");
        }

        if (templatesDir == "" || !Directory.Exists(templatesDir)) {
            throw new RainbowLatinException($"The 'templates.dir' setting in config file 'config.ini' "
                + $"does not contain a valid directory path. Value: '{templatesDir}'.");
        }
    }

    public string GetLatinLemmatizedTextsDir() {
        return latinLemmatizedTextsDir;
    }

    public string GetPerseusCanonicalLatinLitDir() {
        return perseusCanonicalLatinLitDir;
    }

    public string GetWhitakerWordsExecutablePath() {
        return whitakerWordsExecutablePath;
    }

    public string GetOutputDir() {
        return outputDir;
    }

    public string GetTemplatesDir() {
        return templatesDir;
    }
}
