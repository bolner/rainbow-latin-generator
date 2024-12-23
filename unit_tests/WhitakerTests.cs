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
using Xunit;
using RainbowLatinReader;

namespace unit_tests;

public class WhitakerTests
{
    [Fact]
    public void TestEntryParsing1() {
        MockSystemProcess sysProc = new();
        MockLogging logging = new();

        using WhitakerProcess whitProc = new(sysProc, ["exiguae", "amicorum", "qqqqqqqq",
            "copiae", "sunt", "cum", "adversario", "gratiosissimo", "contendat"], logging);
        whitProc.Process();

        var entries = whitProc.GetEntries();

        Assert.True(entries.Count == 8, $"Invalid entry count. Expected 8 but got {entries.Count}.");
        Assert.True(entries.First().GetWord() == "exiguae", $"Invalid first entry. Word expected to be 'exiguae', "
            + $"but got '{entries.First().GetWord()}'.");
    }
}
