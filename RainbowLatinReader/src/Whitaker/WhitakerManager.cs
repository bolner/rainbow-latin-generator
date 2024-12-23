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

namespace RainbowLatinReader;

class WhitakerManager : IWhitakerManager {
    private readonly Dictionary<string, WhitakerEntry> entries = [];
    
    public WhitakerManager(IScheduler<IWhitakerProcess> scheduler,
        HashSet<string> allWords, IConfig config, ILogging logging)
    {
        logging.Print("Starting word lookups in Whitaker's Words.");

        /*
            Looking up dictionary entries.
        */
        var chunks = allWords.Chunk(15);

        foreach(var chunk in chunks) {
            SystemProcess sysProc = new(
                config.GetWhitakerWordsExecutablePath(),
                config.GetWhitakerWordsRootPath()
            );
            scheduler.AddTask(new WhitakerProcess(sysProc, chunk.ToList(), logging));
        }

        scheduler.Run();

        var results = scheduler.GetResults();

        foreach(var result in results) {
            Exception? lastError = result.GetLastError();

            if (lastError != null) {
                throw new RainbowLatinException("Whitaker lookup process failed: "
                    + lastError.Message, lastError);
            }

            var items = result.GetEntries();

            foreach(var item in items) {
                entries[item.GetWord()] = item;
            }

            result.Dispose();
        }

        logging.Print($"Done. Total Latin word count: {allWords.Count}.");
    }

    public WhitakerEntry? GetEntry(string word) {
        WhitakerEntry? entry;
        
        if (!entries.TryGetValue(word, out entry)) {
            return null;
        }

        return entry;
    }
}
