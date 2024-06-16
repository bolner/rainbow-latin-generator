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

class CanonFile : ICanonFile {
    private readonly string path;
    private readonly string documentID;
    private readonly ICanonFile.Language language;
    private readonly int version;
    
    public CanonFile(string path, string documentID,
        ICanonFile.Language language, int version)
    {
        this.path = path;
        this.documentID = documentID;
        this.language = language;
        this.version = version;
    }

    public string GetPath() {
        return path;
    }

    public Stream Open() {
        return File.Open(path, FileMode.Open);
    }

    public string GetDocumentID() {
        return documentID;
    }

    public ICanonFile.Language GetLanguage() {
        return language;
    }

    public int GetVersion() {
        return version;
    }
}
