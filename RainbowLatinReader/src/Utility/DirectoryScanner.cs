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
using System.Text.RegularExpressions;

namespace RainbowLatinReader;

class DirectoryScanner : IDirectoryScanner {
    private readonly Regex analyseIdRegex = new(
        @"([a-z]+[0-9]+\.[a-z]+[0-9]+)\.perseus-(eng|lat)([0-9]+)\.xml$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    private readonly Regex docIdRegex = new(
        @"(stoa|phi)[0-9]{3,5}\.(stoa|phi|abo)[0-9]{3,5}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    private readonly string[] files;
    private int currentFile = 0;
    private readonly ILogging logging;
    private readonly IFileChanges? fileChanges;

    public DirectoryScanner(IEnumerable<string> paths, ILogging logging,
        IFileChanges? fileChanges = null, string? blocklistFilePath = null,
        HashSet<string>? allowedDocumentIDs = null)
    {
        this.logging = logging;
        this.fileChanges = fileChanges;

        /*
            Use blocklist if specified
        */
        if (blocklistFilePath != null) {
            HashSet<string> blocklist = [..
                File.ReadLines(blocklistFilePath)
                    .Select(line => line.Split('\t').First() ?? "")
                    .Where(id => id.Trim() != "")
            ];
            

            bool filter(string x)
            {
                var m = docIdRegex.Match(x);
                bool blocked = blocklist.Contains(m.Value);

                if (blocked)
                {
                    logging.Text("blocked", x);
                }

                return !blocked;
            }

            paths = paths.Where(filter);
        }

        /*
            Filtering by allowed document IDs
            if specified.
        */
        if (allowedDocumentIDs != null) {
            paths = paths.Where((string x) => {
                var m = docIdRegex.Match(x);

                return allowedDocumentIDs.Contains(m.Value);
            });
        }
        
        files = paths.ToArray();
    }

    public ICanonFile? Next() {
        while(files.Length > currentFile) {
            string path = files[currentFile];
            currentFile++;
            string fileName = Path.GetFileName(path);
            int pos = fileName.LastIndexOf(":");
            string name;

            if (pos >= 0) {
                // Lemmatized collection
                name = fileName[(pos + 1)..];
            } else {
                // Canonical Latin Literature
                name = fileName;
            }

            var match = analyseIdRegex.Match(name);
            if (match.Groups.Count < 4) {
                continue;
            }

            string docID = match.Groups[1].Value;
            ICanonFile.Language lang;
            if (match.Groups[2].Value == "lat") {
                lang = ICanonFile.Language.Latin;
            } else {
                lang = ICanonFile.Language.English;
            }
            int version = int.Parse(match.Groups[3].Value);

            /*
                Apply file changes if any.
            */
            List<IFileChangeEntry> changes;
            if (fileChanges == null) {
                changes = [];
            } else {
                changes = fileChanges.Find(fileName).ToList<IFileChangeEntry>();
            }
            
            return new CanonFile(path, docID, lang, version, logging, changes);
        }

        return null;
    }
}
