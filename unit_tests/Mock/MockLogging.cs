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

class MockLogging : ILogging {
    public void Text(string fileName, string text) {

    }

    public void Warning(string fileName, string warning) {

    }

    public void Exception(Exception ex) {

    }

    void IDisposable.Dispose() {

    }

    public void Print(string text) {
        
    }

    public void RegisterUnchangedOutputFile() {

    }

    public void RegisterChangedOutputFile() {

    }

    public int GetUnchangedOutputFileCount() {
        return 0;
    }

    public int GetChangedOutputFileCount() {
        return 0;
    }
}
