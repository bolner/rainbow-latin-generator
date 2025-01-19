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
var fileChangesPaths = Directory.EnumerateFiles(
    Path.Join(dir, "data"), "*.txt", SearchOption.AllDirectories
);
var canonLogging = new Logging(Path.Join(dir, "logs"), "canon");
var fileChangeScanner = new CanonDirectoryScanner(fileChangesPaths, canonLogging);
var fileChanges = new FileChanges(fileChangeScanner);

/*
    Perseus Canonical Literature
*/
var canonPaths = Directory.EnumerateFiles(
    config.GetPerseusCanonicalLatinLitDir(),
    "*.perseus-*.xml",
    SearchOption.AllDirectories
);

var canonScanner = new CanonDirectoryScanner(canonPaths, canonLogging, fileChanges,
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
var lemmaScanner = new CanonDirectoryScanner(lemmaPaths, lemmaLogging, fileChanges: fileChanges,
    allowedDocumentIDs: ids);
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
    Path.Join(dir, "templates", "page.handlebars"), pageLogging
);
var pageManager = new PageManager(pageScheduler, pageLogging, canonLitManager,
    lemmaManager, whitakerManager, pageTemplate,
    Path.Join(dir, "output"));
var indexTemplate = new TemplateEngine(
    Path.Join(dir, "templates", "index.handlebars"), pageLogging
);
pageManager.GenerateIndexPage(indexTemplate,
    Path.Join(dir, "output", "index.html"));
