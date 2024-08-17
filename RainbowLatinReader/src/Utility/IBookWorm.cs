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

interface IBookWorm<ELEMENT_TYPE> {
    public enum ChangeType {
        Add, Remove
    }

    public void IncomingSection(string sectionType, string sectionName);
    public void AddElement(ELEMENT_TYPE element);
    public List<string> GetSectionKeyList();
    public LinkedListNode<ELEMENT_TYPE>? GetFirstNodeBySectionKey(string sectionKey);
    public LinkedListNode<ELEMENT_TYPE>? GetLastNodeBySectionKey(string sectionKey);
    public Dictionary<string, string>? GetSectionValuesForTraceKey(string sectionKey);
    public void ApplyChange(ChangeType changeType, string key, ELEMENT_TYPE content,
        string? after = null, string? before = null);
    public void RemoveSections(List<string> sectionKeys);
}
