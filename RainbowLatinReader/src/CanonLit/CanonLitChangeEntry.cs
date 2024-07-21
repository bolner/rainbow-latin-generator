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

class CanonLitChangeEntry : ICanonLitChangeEntry {
    private readonly ICanonLitChangeEntry.ChangeType changeType;
    private readonly string documentID;
    private readonly ICanonLitChangeEntry.Language language;
    private readonly string? after;
    private readonly string key;
    private readonly string? before;
    private readonly string content;

    public CanonLitChangeEntry(ICanonLitChangeEntry.ChangeType changeType, string documentID,
        ICanonLitChangeEntry.Language language, string? after, string key, string? before,
        string content)
    {
        this.changeType = changeType;
        this.documentID = documentID;
        this.language = language;
        this.after = after;
        this.key = key;
        this.before = before;
        this.content = content;
    }

    public ICanonLitChangeEntry.ChangeType GetChangeType() {
        return changeType;
    }

    public string GetDocumentID() {
        return documentID;
    }

    public ICanonLitChangeEntry.Language GetLanguage() {
        return language;
    }

    public string? GetAfter() {
        return after;
    }

    public string GetKey() {
        return key;
    }

    public string? GetBefore() {
        return before;
    }

    public string GetContent() {
        return content;
    }
}
