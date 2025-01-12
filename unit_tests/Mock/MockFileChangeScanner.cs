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
using System.Text;
using RainbowLatinReader;

namespace unit_tests;

sealed class MockFileChangeScanner : ICanonDirectoryScanner {
    private readonly string content;
    private readonly string path;
    private bool finished = false;

    public MockFileChangeScanner(string content, string path) {
        this.content = content;
        this.path = path;
    }

    public ICanonFile? Next() {
        if (!finished) {
            finished = true;

            return new MockCanonFile(path, "docid", ICanonFile.Language.Latin, 1,
                Encoding.ASCII.GetBytes(content));
        }
        
        return null;
    }
}
