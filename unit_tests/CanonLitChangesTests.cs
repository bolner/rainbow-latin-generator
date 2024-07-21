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
using System.Text;
using Xunit;
using RainbowLatinReader;

namespace unit_tests;

public class CanonLitChangesTests
{
    private readonly byte[] xmlDataLatin = Encoding.ASCII.GetBytes(@"
<changes>
    <add
        documentID=""phi1348.abo011""
        language=""english""
        after=""chapter=48""
        key=""chapter=49""
        before=""chapter=50""
    >
    (Thomson has omitted this chapter.)
    </add>
</changes>
    ");

    [Fact]
    public void TestParsing1()
    {
        var changes = new CanonLitChanges(new MemoryStream(xmlDataLatin));
        var list = changes.Find(ICanonLitChangeEntry.Language.English, "phi1348.abo011");

        Assert.True(list.Count > 0, "Cannot find change for document 'phi1348.abo011'.");
        Assert.True(list.First().GetContent() == "(Thomson has omitted this chapter.)",
            "First change has invalid content.");
    }
}
