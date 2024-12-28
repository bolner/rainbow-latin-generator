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

string dir = Directory.GetCurrentDirectory();
var config = new Config(File.Open(Path.Join(dir, "config.ini"), FileMode.Open));

/*
    Perseus Canonical Literature
*/
var canonPaths = Directory.EnumerateFiles(
    config.GetPerseusCanonicalLatinLitDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
);

var canonLogging = new Logging(Path.Join(dir, "logs"), "canon");
string canonFileChangesPath = Path.Join(dir, "data", "canonLit_changes.txt");
var canonFileChanges = new FileChanges(File.ReadLines(canonFileChangesPath),
    canonFileChangesPath);
var canonScanner = new DirectoryScanner(canonPaths, canonLogging, canonFileChanges,
    Path.Join(dir, "data", "blocklist.tsv"));
var canonScheduler = new Scheduler<ICanonLitDoc>(config.GetThreadCount());
var canonParserFactory = new CanonLitXmlParserFactory();
var canonLitManager = new CanonLitManager(canonScanner, canonScheduler, canonParserFactory,
    canonLogging);

/*
    Lemmatized Latin documents
*/
var lemmaPaths = Directory.EnumerateFiles(
    config.GetLatinLemmatizedTextsDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
);

var lemmaLogging = new Logging(Path.Join(dir, "logs"), "lemma");
var ids = new HashSet<string>(canonLitManager.GetDocumentIDs());
var lemmaScanner = new DirectoryScanner(lemmaPaths, lemmaLogging, allowedDocumentIDs: ids);
var lemmaScheduler = new Scheduler<ILemmatizedDoc>(config.GetThreadCount());
var lemmaParserFactory = new XmlParserFactory();
var lemmaManager = new LemmatizedManager(lemmaScanner, lemmaScheduler,
    lemmaParserFactory, lemmaLogging);

/*
    Whitaker's Words
*/
var whitakerLogging = new Logging(Path.Join(dir, "logs"), "whitaker");
var whitakerScheduler = new Scheduler<IWhitakerProcess>(config.GetThreadCount());
var allWords = canonLitManager.GetAllWords();
var whitakerManager = new WhitakerManager(whitakerScheduler, allWords,
    config, whitakerLogging);

/*
    Pages
*/
var pageLogging = new Logging(Path.Join(dir, "logs"), "page");
var pageScheduler = new Scheduler<IPage>(config.GetThreadCount());
var pageTemplate = new TemplateEngine(
    Path.Join(dir, "templates", "page.handlebars")
);
var pageManager = new PageManager(pageScheduler, pageLogging, canonLitManager,
    lemmaManager, whitakerManager, pageTemplate,
    Path.Join(dir, "output"));
var indexTemplate = new TemplateEngine(
    Path.Join(dir, "templates", "index.handlebars")
);
pageManager.GenerateIndexPage(indexTemplate,
    Path.Join(dir, "output", "index.html"));

pageLogging.Print($"Completed. Total pages: {pageManager.GetDocumentCount()}.");
