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
using HandlebarsDotNet;

namespace RainbowLatinReader;

class TemplateEngine : ITemplateEngine {
    private readonly HandlebarsTemplate<TextWriter, object, object> template;

    public TemplateEngine(string filePath) {
        using var pageTemplateStream = File.OpenText(filePath);
        template = Handlebars.Compile(pageTemplateStream);        
    }

    public void Generate(IDictionary<string, object> data, string outputFilePath) {
        using var outFile = File.CreateText(outputFilePath);
        template(outFile, data);
    }
}
