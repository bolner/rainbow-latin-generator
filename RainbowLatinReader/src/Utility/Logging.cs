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

using System.IO;

sealed class Logging : ILogging {
    private bool isDisposed = false;
    private readonly string logDirectory;
    private readonly string prefix;
    private readonly Dictionary<string, StreamWriter> logFiles = [];

    public Logging(string logDirectory, string prefix) {
        if (!Directory.Exists(logDirectory)) {
            Directory.CreateDirectory(logDirectory);
        }

        this.logDirectory = logDirectory;
        this.prefix = prefix;
    }

    public void Warning(string fileName, string warning) {
        Console.Error.WriteLine($"- {DateTime.Now:HH:mm:ss}: {warning}" + Environment.NewLine);
        AddLogEntry($"{fileName}.log", warning);
    }

    public void Text(string fileName, string text) {
        AddLogEntry($"{fileName}.log", text);
    }

    public void Exception(Exception ex) {
        string text = ex.ToString();
        Console.Error.WriteLine($"- {DateTime.Now:HH:mm:ss}: {text}" + Environment.NewLine);
        AddLogEntry("exception.log", text);
    }

    private void AddLogEntry(string logFile, string text) {
        StreamWriter? f;

        lock(logFiles) {
            if (!logFiles.ContainsKey(logFile)) {
                string path = Path.Join(logDirectory, $"{prefix}_{logFile}");
                File.Delete(path);
                f = File.AppendText(path);
                logFiles[logFile] = f;
            } else {
                f = logFiles[logFile];
            }
            
            f.Write($"- {text}\n\n");
            f.Flush();
        }
    }

    /// <summary>
    /// IDisposable interface
    ///  - We have only managed resources and this class is sealed,
    ///    therefore most of the methods here are not required:
    ///    https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0
    /// </summary>
    public void Dispose() {
        if (!isDisposed) {
            foreach(StreamWriter f in logFiles.Values) {
                f.Close();
            }

            logFiles.Clear();
            isDisposed = true;
        }
    }
}