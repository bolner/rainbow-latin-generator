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

using RainbowLatinReader;

namespace unit_tests;

class MockCanonLitDoc : ICanonLitDoc {
    public string GetEnglishTitle() {
        return "English Title";
    }

    public string GetEnglishAuthor() {
        return "English Author";
    }

    public string GetLatinTitle() {
        return "Latin Title";
    }

    public string GetLatinAuthor() {
        return "Latin Author";
    }

    public string GetEnglishSection(string sectionKey) {
        return $"This is a section of the text ({sectionKey}).";
    }

    public string GetLatinSection(string sectionKey) {
        return $"Hoc est textus segmentum ({sectionKey}).";
    }

    public bool IsExcluded() {
        return false;
    }

    public List<string> GetAllSections() {
        return ["chapter=1|section=1", "chapter=1|section=2",
            "chapter=2|section=1", "chapter=2|section=2"];
    }

    public void Process() {

    }

    public Exception? GetLastError() {
        return null;
    }

    public string GetDocumentID() {
        return "phi1234.phi001";
    }
}
