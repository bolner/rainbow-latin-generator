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

public class DirectoryScannerTests
{
    [Fact]
    public void TestNext1()
    {
        var paths = new List<string>{
            "/tmp/lemmatized/urn:cts:latinLit:phi1276.phi001.perseus-lat2.xml",
            "/tmp/lemmatized/urn:cts:latinLit:stoa0255.stoa008.perseus-eng1.xml",
            "/tmp/phi1348/abo011/phi1348.abo011.perseus-eng2.xml",
            "/tmp/phi0914/phi0012/phi0914.phi0012.perseus-lat3.xml",
        };

        var verifyList = new List<(string, ICanonFile.Language, int)> {
            ("phi1276.phi001", ICanonFile.Language.Latin, 2),
            ("stoa0255.stoa008", ICanonFile.Language.English, 1),
            ("phi1348.abo011", ICanonFile.Language.English, 2),
            ("phi0914.phi0012", ICanonFile.Language.Latin, 3)
        };

        DirectoryScanner scanner = new(paths);
        string lang;

        for(int i = 0; i < 4; i++) {
            var verify = verifyList[i];
            if (verify.Item2 == ICanonFile.Language.Latin) {
                lang = "Latin";
            } else {
                lang = "English";
            }
            var file = scanner.Next();
            Assert.True(file != null, $"Method scanner.Next() returned NULL for path #{i + 1}.");
            Assert.True(file.GetDocumentID() == verify.Item1,
                $"Path #{i + 1} did not resolve to document ID '{verify.Item1}'.");
            Assert.True(file.GetLanguage() == verify.Item2,
                $"Path #{i + 1} did not resolve to language '{lang}'.");
            Assert.True(file.GetVersion() == verify.Item3,
                $"Path #{i + 1} did not resolve to version '{verify.Item3}'.");
        }
    }
}
