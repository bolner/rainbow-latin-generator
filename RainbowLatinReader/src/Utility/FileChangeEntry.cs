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

class FileChangeEntry : IFileChangeEntry {
    private readonly Regex pattern;
    private readonly IFileChangeEntry.ChangeType changeType;
    private readonly string document;
    private readonly string match;
    private readonly string start;
    private readonly string end;
    private readonly string replace;

    public FileChangeEntry(IFileChangeEntry.ChangeType changeType,
        string document, string match, string start,
        string end, string replace)
    {
        this.changeType = changeType;
        this.document = document;
        this.match = match;
        this.start = start;
        this.end = end;
        this.replace = replace;
        
        if (changeType == IFileChangeEntry.ChangeType.SectionReplace) {
            string escStart = Regex.Escape(start);
            string escEnd = Regex.Escape(end);

            // ".*?" for shortest match.
            pattern = new Regex(@$"{escStart}.*?{escEnd}", RegexOptions.Singleline);
        } else {
            pattern = new Regex(Regex.Escape(match), RegexOptions.Singleline);
        }
    }

    /// <summary>
    /// Get the type of the change.
    /// ("String replace" or "section replace".)
    /// </summary>
    public IFileChangeEntry.ChangeType GetChangeType() {
        return changeType;
    }

    /// <summary>
    /// File name of the document.
    /// </summary>
    public string GetDocument() {
        return document;
    }

    /// <summary>
    /// If ChangeType = StringReplace, then
    /// this returns the string to be replaced.
    /// </summary>
    public string GetMatch() {
        return match;
    }

    /// <summary>
    /// The new string.
    /// </summary>
    public string GetReplace() {
        return replace;
    }

    /// <summary>
    /// If ChangeType = SectionReplace, then
    /// this returns the beginning of the section
    /// to be replaced.
    /// </summary>
    public string GetStart() {
        return start;
    }

    /// <summary>
    /// If ChangeType = SectionReplace, then
    /// this returns the end of the section
    /// to be replaced.
    /// </summary>
    public string GetEnd() {
        return end;
    }

    /// <summary>
    /// Replaces the first occurance of the change pattern.
    /// </summary>
    /// <param name="text">Reference to the text. Serves as
    /// both input and output. Will only be overwritten if
    /// the pattern matches.</param>
    /// <returns>True if the pattern matches, false otherwise.</returns>
    public bool Apply(ref string text) {
        bool found = false;

        string tmp = pattern.Replace(text, (match) => {
            found = true;
            return replace;
        } , 1);

        if (found) {
            text = tmp;
        }

        return found;
    }

    /// <summary>
    /// Returns a description of the change for debugging
    /// and error messages.
    /// </summary>
    public override string ToString() {
        if (changeType == IFileChangeEntry.ChangeType.StringReplace) {
            return $"Document: '{document}', Type: StringReplace, Match: '{match}', "
                + $"Replace: '{replace}'.";
        }
        
        return $"Document: '{document}', Type: SectionReplace, Start: '{start}', "
            + $"End: '{end}', Replace: '{replace}'.";
    }
}
