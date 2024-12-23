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

sealed class WhitakerProcess : IWhitakerProcess {
    private bool isDisposed = false;
    private Exception? lastError = null;
    private readonly List<WhitakerEntry> entries = [];
    private readonly ISystemProcess process;
    private readonly string[] words;
    private readonly ILogging logging;

    private static readonly Dictionary<string, string> replace = new() {
        { "á", "a" },
        { "Á", "A" },
        { "â", "a" },
        { "æ", "ae" },
        { "Æ", "AE" },
        { "à", "a" },
        { "ä", "a" },
        { "ç", "c" },
        { "ċ", "c" },
        { "é", "e" },
        { "ê", "e" },
        { "è", "e" },
        { "ē", "e" },
        { "ë", "e" },
        { "Ë", "E" },
        { "í", "I" },
        { "Í", "I" },
        { "î", "I" },
        { "ì", "I" },
        { "ï", "I" },
        { "Ï", "I" },
        { "ñ", "n" },
        { "ó", "o" },
        { "ô", "o" },
        { "œ", "oe" },
        { "Œ", "OE" },
        { "ò", "o" },
        { "ö", "o" },
        { "Ö", "o" },
        { "ŕ", "r" },
        { "ú", "u" },
        { "û", "u" },
        { "ù", "u" },
        { "ü", "u" },
        { "Ü", "U" },
        { "ý", "y" },
        { "ÿ", "y" }
    };

    public WhitakerProcess(ISystemProcess process, List<string> words,
        ILogging logging)
    {
        this.process = process;
        this.words = words.ToArray();
        this.logging = logging;
    }

    public List<WhitakerEntry> GetEntries() {
        return [.. entries];
    }

    public void Process() {
        try {
            /*
                The Whitaker's Words will join some words which are
                common word combinations, into one entry.
                Example: "ita eram".
                To prevent this, the words are separated with an
                'awawaw' delimiter.
            */
            string arguments = string.Join(" awawaw ", words);

            foreach(var pair in replace) {
                if (arguments.Contains(pair.Key)) {
                    arguments = arguments.Replace(pair.Key, pair.Value);
                }
            }
            
            process.Start(arguments);

            string response = process.Read().Trim();
            List<string> lines = response.Split("\n").ToList();;
            lines.Add("");
            List<string> entry = [];
            string trLine = "";
            int wordIndex = 0;
            string word;
            string content;
            bool syncope = false;

            foreach(string line in lines) {
                if (line.Contains("awawaw")) {
                    continue;
                }

                trLine = line.Trim();

                if (trLine.StartsWith("Syncope")) {
                    syncope = true;
                }
                else if (trLine == "" && syncope == false) {
                    if (entry.Count > 0) {
                        if (wordIndex > words.Length - 1) {
                            throw new RainbowLatinException($"Whitaker's words returned more entries ({wordIndex + 1}) "
                                + $"than requested ({words.Length}): "
                                + string.Join(" ", words) + "\n\n" + response);
                        }
                        word = words[wordIndex];
                        content = string.Join('\n', entry);

                        if (!content.Contains("========   UNKNOWN")) {
                            entries.Add(new(word, content));
                        }

                        entry.Clear();
                        wordIndex++;
                    }

                    continue;
                } else {
                    syncope = false;
                }

                entry.Add(trLine);
            }

            if (wordIndex < words.Length) {
                logging.Text("test", response);

                throw new RainbowLatinException($"Whitaker's words returned less entries ({wordIndex}) "
                    + $"than requested ({words.Length}): "
                    + string.Join(" ", words));
            }

            process.WaitForExit();
        } catch(Exception ex) {
            lastError = ex;
            logging.Exception(ex);
        }
    }

    public Exception? GetLastError() {
        return lastError;
    }

    /// <summary>
    /// IDisposable interface
    ///  - We have only managed resources and this class is sealed,
    ///    therefore most of the methods here are not required:
    ///    https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0
    /// </summary>
    public void Dispose() {
        if (!isDisposed) {
            process.Dispose();
            isDisposed = true;
        }
    }
}
