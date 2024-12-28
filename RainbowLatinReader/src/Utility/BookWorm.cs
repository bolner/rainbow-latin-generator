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

    public void ApplyChange(IBookWorm<ELEMENT_TYPE>.ChangeType changeType, string key,
        ELEMENT_TYPE content, string? after = null, string? before = null)
    {
        key = key.Trim();
        if (key == "") {
            throw new RainbowLatinException("A change has empty 'key' value.");
        }

        if (changeType == IBookWorm<ELEMENT_TYPE>.ChangeType.Add) {
            first.TryGetValue(key, out LinkedListNode<ELEMENT_TYPE>? firstNode);
            if (firstNode != null) {
                /*
                    If there are nodes with the given key, then remove them first and
                    insert a single node with the new content.
                */
                last.TryGetValue(key, out LinkedListNode<ELEMENT_TYPE>? lastNode);
                if (lastNode == null) {
                    throw new RainbowLatinException($"BookWorm::ApplyChange(): Missing closing node for key '{key}'.");
                }

                /*
                    Remove sequence from first to last.
                */
                LinkedListNode<ELEMENT_TYPE>? cursor = firstNode;
                LinkedListNode<ELEMENT_TYPE>? next;
                LinkedListNode<ELEMENT_TYPE>? afterLast = lastNode.Next;

                while(cursor != null && cursor != afterLast) {
                    next = cursor.Next;
                    elementChain.Remove(cursor);
                    cursor = next;
                }

                /*
                    Insert new node
                */
                if (afterLast == null) {
                    var node = new LinkedListNode<ELEMENT_TYPE>(content);
                    elementChain.AddLast(node);
                    first[key] = node;
                    last[key] = node;
                } else {
                    var node = new LinkedListNode<ELEMENT_TYPE>(content);
                    elementChain.AddBefore(afterLast, node);
                    first[key] = node;
                    last[key] = node;
                }

                return;
            }

            /*
                If there are no nodes, then insert between the "after"
                and the "before" nodes.
            */
            if (after != null) {
                last.TryGetValue(after, out LinkedListNode<ELEMENT_TYPE>? afterNode);
                if (afterNode == null) {
                    throw new RainbowLatinException($"A change references key '{after}' in an 'after' field. "
                        + "But that key cannot be found in the document.");
                }

                var node = new LinkedListNode<ELEMENT_TYPE>(content);
                elementChain.AddAfter(afterNode, node);
                first[key] = node;
                last[key] = node;
            } else if (before != null) {
                first.TryGetValue(before, out LinkedListNode<ELEMENT_TYPE>? beforeNode);
                if (beforeNode == null) {
                    throw new RainbowLatinException($"A change references key '{before}' in a 'before' field. "
                        + "But that key cannot be found in the document.");
                }

                var node = new LinkedListNode<ELEMENT_TYPE>(content);
                elementChain.AddBefore(beforeNode, node);
                first[key] = node;
                last[key] = node;
            } else {
                throw new RainbowLatinException("Both the 'after' and 'before' fields are unset for a change "
                    + "for which the 'key' was not found in the document.");
            }

            return;
        }

        // TODO: remove
    }

    public void RemoveSections(List<string> sectionKeys) {
        foreach(string key in sectionKeys) {
            first.TryGetValue(key, out LinkedListNode<ELEMENT_TYPE>? firstNode);
            if (firstNode != null) {
                /*
                    If there are nodes with the given key, then remove them first and
                    insert a single node with the new content.
                */
                last.TryGetValue(key, out LinkedListNode<ELEMENT_TYPE>? lastNode);
                if (lastNode == null) {
                    throw new RainbowLatinException($"BookWorm::ApplyChange(): Missing closing node for key '{key}'.");
                }

                /*
                    Remove sequence from first to last.
                */
                LinkedListNode<ELEMENT_TYPE>? cursor = firstNode;
                LinkedListNode<ELEMENT_TYPE>? next;
                LinkedListNode<ELEMENT_TYPE>? afterLast = lastNode.Next;

                while(cursor != null && cursor != afterLast) {
                    next = cursor.Next;
                    elementChain.Remove(cursor);
                    cursor = next;
                }

                /*
                    Remove references
                */
                first.Remove(key);
                last.Remove(key);
            }
        }
    }
}
