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

using System.Diagnostics;

namespace RainbowLatinReader;

sealed class SystemProcess : ISystemProcess {
    private bool isDisposed = false;
    private readonly Process process;

    public SystemProcess(string execPath, string workDir) {
        ProcessStartInfo startInfo = new()
        {
            WorkingDirectory = workDir,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            FileName = execPath
        };

        process = new()
        {
            StartInfo = startInfo
        };
    }

    public void Start(string arguments) {
        process.StartInfo.Arguments = arguments;
        process.Start();
    }

    public string Read() {
        return process.StandardOutput.ReadToEnd();
    }

    public void Write(string data) {
        process.StandardInput.Write(data);
        process.StandardInput.Flush();
    }

    public void WaitForExit() {
        process.WaitForExit();
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
