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
using System.Text;

namespace RainbowLatinReader;

class CanonFile : ICanonFile {
    private readonly string path;
    private readonly string fileName;
    private readonly string documentID;
    private readonly ICanonFile.Language language;
    private readonly int version;
    private readonly List<IFileChangeEntry> changes;
    private readonly ILogging logging;

    private static readonly Dictionary<string, string> replace = new() {
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
    
    public CanonFile(string path, string documentID,
        ICanonFile.Language language, int version,
        ILogging logging, List<IFileChangeEntry> changes)
    {
        this.path = path;
        this.documentID = documentID;
        this.language = language;
        this.version = version;
        this.changes = changes;
        this.logging = logging;

        fileName = Path.GetFileName(path);
    }

    public string GetPath() {
        return path;
    }

    public Stream Open() {
        string text = File.ReadAllText(path, Encoding.UTF8);

        foreach(var pair in replace) {
            if (text.Contains(pair.Key)) {
                text = text.Replace(pair.Key, pair.Value);
            }
        }

        foreach(var change in changes) {
            if (change.Apply(ref text)) {
                logging.Text("changes", $"Change successful: {change}");
            } else {
                logging.Text("changes", $"Change failed, no match: {change}");
            }
        }
        
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }

    public string GetDocumentID() {
        return documentID;
    }

    public ICanonFile.Language GetLanguage() {
        return language;
    }

    public int GetVersion() {
        return version;
    }
}
