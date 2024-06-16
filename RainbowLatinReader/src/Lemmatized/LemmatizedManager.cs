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

class LemmatizedManager : ILemmatizedManager {
    private readonly Dictionary<string, ILemmatizedDoc> library = new();

    public LemmatizedManager(IDirectoryScanner scanner,
        IScheduler<ILemmatizedDoc> scheduler,
        IXmlParserFactory xmlParserFactory)
    {
        ICanonFile? file;

        /*
            Schedule all parsing tasks
        */
        while((file = scanner.Next()) != null) {
            scheduler.AddTask(new LemmatizedDoc(file, xmlParserFactory));
        }

        /*
            Parse documents and store results, indexed.
        */
        scheduler.Run();

        foreach(var doc in scheduler.GetResults()) {
            library[doc.GetDocumentID()] = doc;
        }
    }

    public ILemmatizedDoc GetDocument(string documentID) {
        ILemmatizedDoc? value = null;
        library.TryGetValue(documentID, out value);
        if (value == null) {
            throw new RainbowLatinException($"Cannot find CanonLit document with ID '{documentID}'.");
        }

        return value;
    }
}
