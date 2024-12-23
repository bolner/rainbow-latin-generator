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

interface ICanonLitDoc : IProcessable {
    public string GetEnglishTitle();
    public string GetEnglishAuthor();
    public string GetLatinTitle();
    public string GetLatinAuthor();
    public string GetEnglishSection(string sectionKey);
    public string GetLatinSection(string sectionKey);
    public bool IsExcluded();
    public List<string> GetAllSections();
    public string GetDocumentID();
}
