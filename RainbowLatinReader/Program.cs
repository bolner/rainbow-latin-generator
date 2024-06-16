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

var config = new Config(File.Open(Path.Join(Directory.GetCurrentDirectory(),
    "config.ini"), FileMode.Open));
var allowlist = new List<string> { "phi1348.abo011" };
var filter = (string x) => {
    return true;
    /*foreach(string allow in allowlist) {
        if (x.Contains(allow)) {
            Console.WriteLine(" - " + x);
            return true;
        }
    }
    
    return false;*/
};

/*
    Perseus Canonical Literature
*/
var canonPaths = Directory.EnumerateFiles(
    config.GetPerseusCanonicalLatinLitDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
).Where(filter);

var canonScanner = new DirectoryScanner(canonPaths);
var canonScheduler = new Scheduler<ICanonLitDoc>(1);
var canonParserFactory = new XmlParserFactory();
var canonLitManager = new CanonLitManager(canonScanner, canonScheduler, canonParserFactory);

/*
    Lemmatized Latin documents
*/
var lemmaPaths = Directory.EnumerateFiles(
    config.GetLatinLemmatizedTextsDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
).Where(filter);

var lemmaScanner = new DirectoryScanner(lemmaPaths);
var lemmaScheduler = new Scheduler<ILemmatizedDoc>(1);
var lemmaParserFactory = new XmlParserFactory();
var lemmaManager = new LemmatizedManager(lemmaScanner, lemmaScheduler, lemmaParserFactory);
