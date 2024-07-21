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

using System.Xml;

class CanonLitChanges : ICanonLitChanges {
    private readonly Dictionary<string, List<CanonLitChangeEntry>> lookup = [];

    public CanonLitChanges(Stream xmlContent) {
        using var reader = XmlReader.Create(xmlContent, new XmlReaderSettings() {
            DtdProcessing = DtdProcessing.Ignore
        });

        ICanonLitChangeEntry.ChangeType? changeType = null;
        ICanonLitChangeEntry.Language? language = null;
        string? documentID = null;
        string? after = null;
        string? key = null;
        string? before = null;
        string? content = null;

        while(!reader.EOF) {
            if (!reader.Read()) {
                break;
            }

            if (reader.NodeType == XmlNodeType.Text) {
                if (changeType == ICanonLitChangeEntry.ChangeType.Add && language != null
                    && documentID != null && after != null
                    && key != null && before != null)
                {
                    content = (reader.ReadContentAsString() ?? "").Trim();

                    lookup.TryGetValue(documentID, out var list);
                    if (list == null) {
                        list = [];
                        lookup[documentID] = list;
                    }

                    list.Add(new CanonLitChangeEntry(
                        (ICanonLitChangeEntry.ChangeType)changeType, documentID,
                        (ICanonLitChangeEntry.Language)language, after, key, before, content
                    ));
                }
            } else if (reader.NodeType == XmlNodeType.Element) {
                changeType = null;
                language = null;
                documentID = null;
                after = null;
                key = null;
                before = null;
                content = null;
                
                if (reader.Name == "add") {
                    changeType = ICanonLitChangeEntry.ChangeType.Add;
                } else if (reader.Name == "remove") {
                    changeType = ICanonLitChangeEntry.ChangeType.Remove;
                } else {
                    continue;
                }

                Dictionary<string, string> attributes = [];

                while (reader.MoveToNextAttribute()) {
                    attributes[reader.Name] = reader.Value;
                }

                attributes.TryGetValue("documentID", out documentID);
                if (documentID == null) {
                    continue;
                }

                attributes.TryGetValue("language", out string? langStr);
                if (langStr == "latin") {
                    language = ICanonLitChangeEntry.Language.Latin;
                } else if (langStr == "english") {
                    language = ICanonLitChangeEntry.Language.English;
                } else {
                    continue;
                }

                attributes.TryGetValue("after", out after);
                attributes.TryGetValue("before", out before);
                attributes.TryGetValue("key", out key);
                if (key == null) {
                    continue;
                }
            }
        }
    }

    public List<CanonLitChangeEntry> Find(ICanonLitChangeEntry.Language language, string documentID,
        ICanonLitChangeEntry.ChangeType? changeType = null)
    {
        lookup.TryGetValue(documentID, out var list);
        if (list == null) {
            return [];
        }

        if (changeType == null) {
            return list.Where(x => x.GetLanguage() == language).ToList();
        }
        
        return list.Where(x => x.GetLanguage() == language
            && x.GetChangeType() == changeType).ToList();
    }
}
