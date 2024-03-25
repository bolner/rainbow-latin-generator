using System;

args = Environment.GetCommandLineArgs();

if (args.Length < 2) {
    Console.WriteLine("Using: dotnet run [Canonical root directory]");
    Environment.Exit(0);
}

string dir = args[1];

if (!Directory.Exists(dir)) {
    Console.Error.WriteLine("First parameter must be a path to a directory.");
    Environment.Exit(1);
}

Dictionary<string, string> replace = new() {
    { "&aacute;", "á" },
    { "&Aacute;", "Á" },
    { "&acirc;", "â" },
    { "&aelig;", "æ" },
    { "&AElig;", "Æ" },
    { "&agrave;", "à" },
    { "&auml;", "ä" },
    { "&ccedil;", "ç" },
    { "&cdot;", "ċ" },
    { "&dagger;", "†" },
    { "&deg;", "°" },
    { "&eacute;", "é" },
    { "&ecirc;", "ê" },
    { "&egrave;", "è" },
    { "&emacr;", "ē" },
    { "&euml;", "ë" },
    { "&Euml;", "Ë" },
    { "&iacute;", "í" },
    { "&Iacute;", "Í" },
    { "&icirc;", "î" },
    { "&igrave;", "ì" },
    { "&iuml;", "ï" },
    { "&Iuml;", "Ï" },
    { "&ldquo;", "“" },
    { "&lsquo;", "‘" },
    { "&mdash;", "—" },
    { "&ndash;", "–" },
    { "&ntilde;", "ñ" },
    { "&oacute;", "ó" },
    { "&ocirc;", "ô" },
    { "&oelig;", "œ" },
    { "&OElig;", "Œ" },
    { "&ograve;", "ò" },
    { "&ouml;", "ö" },
    { "&Ouml;", "Ö" },
    { "&pound;", "£" },
    { "&prime;", "′" },
    { "&racute;", "ŕ" },
    { "&rdquo;", "”" },
    { "&rsquo;", "’" },
    { "&sect;", "§" },
    { "&uacute;", "ú" },
    { "&ucirc;", "û" },
    { "&ugrave;", "ù" },
    { "&uuml;", "ü" },
    { "&Uuml;", "Ü" },
    { "&yacute;", "ý" },
    { "&yuml;", "ÿ" },
    { "&Perseus.publish;", "" }
};

var canonPaths = Directory.EnumerateFiles(
    dir,
    "*.perseus-*.xml",
    SearchOption.AllDirectories
);

foreach(string path in canonPaths) {
    string text = File.ReadAllText(path);

    foreach(var pair in replace) {
        text = text.Replace(pair.Key, pair.Value);
    }

    Console.WriteLine(path);
    File.WriteAllText(path, text);
}
