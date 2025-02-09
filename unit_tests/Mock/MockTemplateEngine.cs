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

class MockTemplateEngine : ITemplateEngine {
    private readonly Dictionary<string, IDictionary<string, object>> library = [];

    public void Generate(IDictionary<string, object> data, string outputFilePath) {
        library[outputFilePath] = data;
    }

    public IDictionary<string, object>? GetDataForPath(string outputFilePath) {
        if (!library.ContainsKey(outputFilePath)) {
            return null;
        }

        return library[outputFilePath];
    }
}
