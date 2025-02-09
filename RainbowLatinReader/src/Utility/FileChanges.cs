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

class FileChanges : IFileChanges {
    private readonly Dictionary<string, List<FileChangeEntry>> changes = [];
    private readonly Regex labelRegex = new(@"^(document|match|regex|replace|start|end)\: (.*)$");

    /// <summary>
    /// Constructor. Parses a file with change requests.
    /// </summary>
    /// <param name="lines">The file content to parse.</param>
    /// <param name="filePath">Used only for error messages.</param>
    public FileChanges(ICanonDirectoryScanner scanner) {
        string? lastLabel = null;
        Dictionary<string, string> state = [];
        List<string> parts = [];
        int lineNum;

        void SectionEnded(string filePath) {
            if (lastLabel != null) {
                state[lastLabel] = string.Join(' ', parts);
                parts.Clear();
                lastLabel = null;
            }

            if (state.ContainsKey("document") && state.ContainsKey("match")
                && state.ContainsKey("replace"))
            {
                if (!changes.ContainsKey(state["document"])) {
                    changes[state["document"]] = [];
                }

                changes[state["document"]].Add(new(IFileChangeEntry.ChangeType.StringReplace,
                    state["document"], state["match"], "", "", state["replace"]));
            }
            else if (state.ContainsKey("document") && state.ContainsKey("start")
                && state.ContainsKey("end") && state.ContainsKey("replace"))
            {
                if (!changes.ContainsKey(state["document"])) {
                    changes[state["document"]] = [];
                }

                changes[state["document"]].Add(new(IFileChangeEntry.ChangeType.SectionReplace,
                    state["document"], "", state["start"], state["end"], state["replace"]));
            }
            else if (state.ContainsKey("document") && state.ContainsKey("regex")
                && state.ContainsKey("replace"))
            {
                if (!changes.ContainsKey(state["document"])) {
                    changes[state["document"]] = [];
                }
                
                changes[state["document"]].Add(new(IFileChangeEntry.ChangeType.RegEx,
                    state["document"], state["regex"], "", "", state["replace"]));
            }
            else if (state.Count > 0)
            {
                throw new RainbowLatinException($"Invalid fileChanges file '{filePath}' on LINE {lineNum}: "
                    + "Previous section is missing labels.");
            }

            state.Clear();
        }

        ICanonFile? file;
        string? line;

        while((file = scanner.Next()) != null) {
            using var stream = file.Open();
            using StreamReader reader = new(stream);
            lineNum = 0;
            
            while((line = reader.ReadLine()) != null) {
                lineNum++;

                if (line.StartsWith('#')) {
                    // Comment lines start with #.
                    continue;
                }

                if (line.Trim() == "") {
                    SectionEnded(file.GetPath());
                    continue;
                }

                Match match = labelRegex.Match(line);
                if (match.Groups.Count < 3) {
                    parts.Add(line.Trim());
                } else {
                    if (lastLabel != null) {
                        state[lastLabel] = string.Join(' ', parts);
                    }
                    
                    parts.Clear();
                    lastLabel = match.Groups[1].Value;
                    parts.Add(match.Groups[2].Value);
                }
            }

            SectionEnded(file.GetPath());
        }
    }

    public List<FileChangeEntry> Find(string fileName) {
        if (!changes.ContainsKey(fileName)) {
            return [];
        }

        return [.. changes[fileName]];
    }
}
