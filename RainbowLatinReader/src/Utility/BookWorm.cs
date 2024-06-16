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
    private readonly List<string> trace = []; // Section types
    private readonly Dictionary<string, string> traceValues = []; // Section names for each type
    private string currentSectionKey = "na";
    private readonly List<string> sectionList = [];
    private readonly LinkedList<ELEMENT_TYPE> elementChain = [];
    private readonly Dictionary<string, LinkedListNode<ELEMENT_TYPE>> first = [];
    private readonly Dictionary<string, LinkedListNode<ELEMENT_TYPE>> last = [];

    /// <summary>
    /// Constructor.
    /// </summary>
    public BookWorm() {
        
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

        // Detect if 'sectionType' is on trace, if yes, then rewind only.
        if (trace.Contains(sectionType)) {
            while(trace.Count > 0 && trace.Last() != sectionType) {
                traceValues.Remove(trace.Last());
                trace.RemoveAt(trace.Count - 1);
            }
        } else {
            trace.Add(sectionType);
        }

        traceValues[sectionType] = sectionName;
        currentSectionKey = GetSectionKey();

        if (first.ContainsKey(currentSectionKey)) {
            throw new RainbowLatinException("Invalid section structure. Section trace found twice: "
                + currentSectionKey);
        }
    }

    public void AddElement(ELEMENT_TYPE element) {
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

        foreach(string section in trace) {
            parts.Add($"{section}={traceValues[section]}");
        }

        return string.Join('|', parts);
    }

    public List<string> GetSectionKeyList() {
        return new List<string>(sectionList);
    }

    public LinkedListNode<ELEMENT_TYPE>? GetFirstNodeBySectionKey(string sectionKey) {
        if (!first.TryGetValue(sectionKey, out LinkedListNode<ELEMENT_TYPE>? value)) {
            return null;
        }

        return value;
    }

    public LinkedListNode<ELEMENT_TYPE>? GetLastNodeBySectionKey(string sectionKey) {
        if (!last.TryGetValue(sectionKey, out LinkedListNode<ELEMENT_TYPE>? value)) {
            return null;
        }

        return value;
    }
}
