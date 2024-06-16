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
