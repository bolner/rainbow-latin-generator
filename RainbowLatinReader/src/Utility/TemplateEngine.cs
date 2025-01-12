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
using System.Text.RegularExpressions;
using HandlebarsDotNet;

namespace RainbowLatinReader;

class TemplateEngine : ITemplateEngine {
    private readonly HandlebarsTemplate<TextWriter, object, object> template;
    private readonly Regex genMatch = new(
        @"Generated on [0-9]{4,4}\-[0-9]{2,2}\-[0-9]{2,2}",
        RegexOptions.Compiled);
    private readonly ILogging logging;

    public TemplateEngine(string filePath, ILogging logging) {
        using var pageTemplateStream = File.OpenText(filePath);
        template = Handlebars.Compile(pageTemplateStream);        
        this.logging = logging;
    }

    public void Generate(IDictionary<string, object> data, string outputFilePath) {
        using MemoryStream ms = new();
        using StreamWriter writer = new(ms);

        template(writer, data);
        writer.Close();

        string newContent = Encoding.UTF8.GetString(ms.ToArray());
        
        /*
            Detect if the "Generated on" date is the only change.
            If yes, then dont update the file.
        */
        if (File.Exists(outputFilePath)) {
            string oldContent = File.ReadAllText(outputFilePath);

            if (genMatch.Replace(oldContent, "") == genMatch.Replace(newContent, "")) {
                logging.RegisterUnchangedOutputFile();
                return;
            }
        }

        File.WriteAllText(outputFilePath, newContent);
        logging.RegisterChangedOutputFile();
    }
}
