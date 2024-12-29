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

namespace RainbowLatinReader;

class BookWorm<ELEMENT_TYPE> : IBookWorm<ELEMENT_TYPE> {
    private readonly Dictionary<string, string> sections = [];
    private string currentSectionKey = "na";
    private readonly List<string> sectionList = [];
    private readonly LinkedList<ELEMENT_TYPE> elementChain = [];
    private readonly Dictionary<string, LinkedListNode<ELEMENT_TYPE>> first = [];
    private readonly Dictionary<string, LinkedListNode<ELEMENT_TYPE>> last = [];
    private readonly Dictionary<string, Dictionary<string, string>> traceKeyToValues = [];
    private readonly List<string> sectionHierarchy = ["book", "letter", "speech", "chapter", "poem", "section",
        "card", "paragraph", "para"];
    private readonly bool clearLowerLevels;
    
    /// <summary>
    /// Constructor.
    /// </summary>
    public BookWorm(bool clearLowerLevels = false) {
        this.clearLowerLevels = clearLowerLevels;
    }

    /// <summary>
    /// Reached a milestone or a div/textpart, etc.
    /// </summary>
    /// <param name="sectionType">Examples: "section", "chapter", "para", ...</param>
    /// <param name="sectionName">Examples: "1", "2", ...</param>
    public void IncomingSection(string sectionType, string sectionName) {
        if (sectionType.Contains('=') || sectionType.Contains('|')) {
            throw new RainbowLatinException("The section type is not allowed to contain the "
                + $"characters: '=', '|'. Received: '{sectionType}'");
        }

        if (sectionName.Contains('=') || sectionName.Contains('|')) {
            throw new RainbowLatinException("The section name is not allowed to contain the "
                + $"characters: '=', '|'. Received: '{sectionName}'");
        }

        sections.TryGetValue(sectionType, out string? oldName);
        if (oldName == sectionName) {
            // No change (Mostly a repetition of a section mark.)
            return;
        }

        /*
            If a higher level is encountered, then clear the lower levels.
        */
        if (clearLowerLevels) {
            bool clear = false;

            foreach(var level in sectionHierarchy) {
                if (level == sectionType) {
                    clear = true;
                    continue;
                }

                if (clear) {
                    sections.Remove(level);
                }
            }
        }

        /*
            Register and validate new section.
        */
        if (sectionName != "") {
            sections[sectionType] = sectionName;
        }
        currentSectionKey = GetSectionKey();

        if (first.ContainsKey(currentSectionKey)) {
            throw new RainbowLatinException("Invalid section structure. Section trace found twice: "
                + $"{currentSectionKey}. The old one starts with: '{first[currentSectionKey].Value}'. "
                + $"Incoming section type: '{sectionType}'.");
        }

        traceKeyToValues[currentSectionKey] = new Dictionary<string, string>(sections);
    }

    public void AddElement(ELEMENT_TYPE element) {
        if (currentSectionKey == "na") {
            return;
        }

        var node = new LinkedListNode<ELEMENT_TYPE>(element);

        // is it the first element after a new section?
        if (!first.ContainsKey(currentSectionKey)) {
            first[currentSectionKey] = node;
            last[currentSectionKey] = node;

            sectionList.Add(currentSectionKey);
        } else {
            // Always overwrite the "last", so the last will stay there.
            last[currentSectionKey] = node;
        }

        elementChain.AddLast(node);
    }

    private string GetSectionKey() {
        List<string> parts = [];

        foreach(var item in sections) {
            parts.Add($"{item.Key}={item.Value}");
        }

        parts.Sort();

        return string.Join('|', parts);
    }

    public List<string> GetSectionKeyList() {
        return new List<string>(first.Keys);
    }

    public LinkedListNode<ELEMENT_TYPE>? GetFirstNodeBySectionKey(string sectionKey) {
        sectionKey = CanonizeSectionKey(sectionKey);
        if (!first.TryGetValue(sectionKey, out LinkedListNode<ELEMENT_TYPE>? value)) {
            return null;
        }

        return value;
    }

    public LinkedListNode<ELEMENT_TYPE>? GetLastNodeBySectionKey(string sectionKey) {
        sectionKey = CanonizeSectionKey(sectionKey);
        if (!last.TryGetValue(sectionKey, out LinkedListNode<ELEMENT_TYPE>? value)) {
            return null;
        }

        return value;
    }

    public Dictionary<string, string>? GetSectionValuesForTraceKey(string sectionKey) {
        sectionKey = CanonizeSectionKey(sectionKey);
        if (!traceKeyToValues.TryGetValue(sectionKey, out Dictionary<string, string>? value)) {
            return null;
        }

        return value;
    }

    private static string CanonizeSectionKey(string sectionKey) {
        var parts = sectionKey.Split('|').ToList();
        parts.Sort();

        return string.Join('|', parts);
    }
}
