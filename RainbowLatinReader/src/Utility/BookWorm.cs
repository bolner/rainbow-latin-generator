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
    private readonly Dictionary<string, string> pivotSection = [];
    private readonly Dictionary<string, string> floatingSection = [];
    private readonly List<ELEMENT_TYPE> floatingElements = [];
    private readonly LinkedList<ELEMENT_TYPE> elementChain = [];
    private readonly Dictionary<string, LinkedListNode<ELEMENT_TYPE>> first = [];
    private readonly Dictionary<string, LinkedListNode<ELEMENT_TYPE>> last = [];
    private readonly Dictionary<string, Dictionary<string, string>> traceKeyToValues = [];
    private readonly List<string> sectionHierarchy = ["book", "letter", "speech", "act", "chapter", "actio",
        "scene", "poem", "section", "alternatesection", "card", "paragraph", "para"];
    private readonly Dictionary<string, int> sectionHierarchyNumeric = [];
    
    /// <summary>
    /// Constructor.
    /// </summary>
    public BookWorm() {
        int i = sectionHierarchy.Count;

        foreach(string sectionType in sectionHierarchy) {
            sectionHierarchyNumeric[sectionType] = i;
            i--;
        }
    }

    /// <summary>
    /// Reached a milestone or a div/textpart, etc.
    /// See: "doc/BookWorm cases.md"
    /// </summary>
    /// <param name="sectionType">Examples: "section", "chapter", "para", ...</param>
    /// <param name="sectionName">Examples: "1", "2", ...</param>
    public void IncomingSection(string sectionType, string sectionName)
    {
        if (sectionType.Contains('=') || sectionType.Contains('|')) {
            throw new RainbowLatinException("The section type is not allowed to contain the "
                + $"characters: '=', '|'. Received: '{sectionType}'");
        }

        if (sectionName.Contains('=') || sectionName.Contains('|')) {
            throw new RainbowLatinException("The section name is not allowed to contain the "
                + $"characters: '=', '|'. Received: '{sectionName}'");
        }

        if (!sectionHierarchyNumeric.TryGetValue(sectionType, out int newLevel)) {
            throw new RainbowLatinException($"Unknown section type: {sectionType}.");
        }

        floatingSection.TryGetValue(sectionType, out string? oldName);
        if (oldName == sectionName) {
            // No change (Mostly a repetition of a section marker.)
            return;
        }

        /*
            "We encounter a section marker which is already present
            in 'floating section' and would set the level to a new value."
        */
        if (oldName != null && oldName != sectionName) {
            SectionCompletion();
            floatingSection[sectionType] = sectionName;
            return;
        }

        /*
            "A newly encountered marker has higher level than any previous markers in 'floating section',
            AND that marker is persent in the pivot (meaning: not reverse-floating.)
            AND at least one element (text) appeared in the current section."
        */
        if (pivotSection.ContainsKey(sectionType) && floatingElements.Count > 0) {
            bool foundHigher = false;

            foreach(string key in floatingSection.Keys) {
                if (sectionHierarchyNumeric[key] > newLevel) {
                    foundHigher = true;
                    break;
                }
            }

            if (!foundHigher) {
                SectionCompletion();
                floatingSection[sectionType] = sectionName;
                return;
            }
        }

        /*
            No completion. Just add the new marker to
            the floating section.
        */
        floatingSection[sectionType] = sectionName;
    }

    public void AddElement(ELEMENT_TYPE element) {
        floatingElements.Add(element);
    }

    public void EndOfDocument() {
        SectionCompletion();
    }

    private void SectionCompletion() {
        foreach(var item in floatingSection) {
            pivotSection[item.Key] = item.Value;
        }

        floatingSection.Clear();
        string currentSectionKey = GetPivotSectionKey();

        if (first.ContainsKey(currentSectionKey)) {
            throw new RainbowLatinException("Invalid section structure. Section trace found twice: "
                + $"{currentSectionKey}. The old one starts with: '{first[currentSectionKey].Value}'. ");
        }

        traceKeyToValues[currentSectionKey] = new Dictionary<string, string>(pivotSection);

        /*
            Store the floating elements under the new pivot section.
        */
        foreach(ELEMENT_TYPE element in floatingElements) {
            var node = new LinkedListNode<ELEMENT_TYPE>(element);

            // is it the first element after a new section?
            if (!first.ContainsKey(currentSectionKey)) {
                first[currentSectionKey] = node;
                last[currentSectionKey] = node;
            } else {
                // Always overwrite the "last", so the last will stay there.
                last[currentSectionKey] = node;
            }

            elementChain.AddLast(node);
        }

        floatingElements.Clear();
    }

    private string GetPivotSectionKey() {
        List<string> parts = [];

        foreach(var item in pivotSection) {
            parts.Add($"{item.Key}={item.Value}");
        }

        parts.Sort();

        return string.Join('|', parts);
    }

    public List<string> GetSectionKeyList() {
        return [.. first.Keys];
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
