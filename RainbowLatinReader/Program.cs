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
using System.Text.RegularExpressions;
using RainbowLatinReader;

var config = new Config(File.Open(Path.Join(Directory.GetCurrentDirectory(),
    "config.ini"), FileMode.Open));
var canonLogging = new Logging(Path.Join(Directory.GetCurrentDirectory(), "logs"), "canon");
HashSet<string> blocklist = [
    "stoa0054.stoa006", // https://github.com/PerseusDL/canonical-latinLit/issues/572
    "stoa0040.stoa011", // Weird structure in stoa0040.stoa011.perseus-eng1.xml
    "phi0914.phi0011", // Issue with chapters: phi0914.phi0011.perseus-eng2.xml
    "phi0478.phi003", // English document is partial: data/phi0478/phi003/phi0478.phi003.perseus-eng1.xml
    "phi0845.phi002", // The English doc breaks the text into more sections than the Latin: phi0845.phi002.perseus-eng1.xml
    "phi0690.phi002", // It is line-based. Line between English-Latin don't match.
    "phi0914.phi001", // Too many sections are missing from the English translation
    "phi0474.phi056", // Irregular section names in the English docs
    "phi0474.phi057", // Irregular section names in the English docs
    "phi0474.phi058", // Irregular section names in the English docs
    "phi0474.phi059", // Irregular section names in the English docs
];
var rg = new Regex(@"(stoa|phi)[0-9]{3,5}\.(stoa|phi|abo)[0-9]{3,5}");

var filter = (string x) => {
    var m = rg.Match(x);
    
    return !blocklist.Contains(m.Value);
};

var canonLitChanges = new CanonLitChanges(File.Open(Path.Join(Directory.GetCurrentDirectory(),
    "data", "canonLit_changes.xml"), FileMode.Open));

/*
    Perseus Canonical Literature
*/
var canonPaths = Directory.EnumerateFiles(
    config.GetPerseusCanonicalLatinLitDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
).Where(filter);

var canonScanner = new DirectoryScanner(canonPaths);
var canonScheduler = new Scheduler<ICanonLitDoc>(44);
var canonParserFactory = new XmlParserFactory();
var canonLitManager = new CanonLitManager(canonScanner, canonScheduler, canonParserFactory,
    canonLitChanges, canonLogging);

/*
    Lemmatized Latin documents
*/
var lemmaLogging = new Logging(Path.Join(Directory.GetCurrentDirectory(), "logs"), "lemma");
var ids = new HashSet<string>(canonLitManager.GetDocumentIDs());
var lemmaPaths = Directory.EnumerateFiles(
    config.GetLatinLemmatizedTextsDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
).Where((string x) => {
    var m = rg.Match(x);

    return ids.Contains(m.Value);
});

var lemmaScanner = new DirectoryScanner(lemmaPaths);
var lemmaScheduler = new Scheduler<ILemmatizedDoc>(44);
var lemmaParserFactory = new XmlParserFactory();
var lemmaManager = new LemmatizedManager(lemmaScanner, lemmaScheduler, lemmaParserFactory,
    lemmaLogging);
