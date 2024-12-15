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

public class FileChangesTests
{
    private readonly string fileChanges = @"
document: phi0631.phi001.perseus-eng2.xml
match: Lucius Catiline to Quintus Catulus, wishing health. Your eminent integrity, known to me by experience
replace: Lucius Catiline to Quintus Catulus, wishing health.<div>
    <div type=""textpart"" subtype=""chapter"" n=""35"">
    Your eminent integrity, known to me by experience

document: phi0631.phi001.perseus-eng2.xml
match: <p>LVII When it was reported in his camp, however, that the conspiracy
replace: <div><div type=""textpart"" subtype=""chapter"" n=""57"">
    <p>LVII When it was reported in his camp, however, that the conspiracy

document: phi1348.abo011.perseus-eng2.xml
start: <div type=""textpart"" n=""49"" subtype=""chapter"">
end: </div>
replace: <div type=""textpart"" n=""49"" subtype=""chapter"">
    (Thomson has omitted this chapter.)
    </div>
    ";

    private readonly string simpleChanges = @"
document: string_replace.xml
match: Replace this.
replace: With this.

document: section_replace.xml
start: start with this
end: this ends here
replace: XXXXXXXXXXXXXXXXXXX
    YYYYYYYYYYYYYYYYYYYY
    ZZZZZZZZZZZZZZZ
    ";

    [Fact]
    public void TestParsing1()
    {
        var changes = new FileChanges(fileChanges.Split('\n'), "/tmp/canonLit_changes.txt");
        var list = changes.Find("phi0631.phi001.perseus-eng2.xml");

        Assert.True(list.Count == 2, "Cannot find 2 changes for document 'phi0631.phi001.perseus-eng2.xml'.");
    }

    [Fact]
    public void TestStringReplace1()
    {
        var changes = new FileChanges(simpleChanges.Split('\n'), "/tmp/canonLit_changes.txt");
        var list = changes.Find("string_replace.xml");

        Assert.True(list.Count == 1, "Cannot find simple string replace example.");
        
        string text = @"
The quick brown fox jumps over the lazy dog.
Replace this.
Sphinx of black quartz, judge my vow.
        ".Trim();

        foreach(var change in list) {
            bool found = change.Apply(ref text);

            Assert.True(found, "Pattern did not match in simple string replace example.");
            Assert.True(text.Contains("With this."), "Pattern was not replaced in simple string replace example.");
        }
    }

    [Fact]
    public void TestSectionReplace1()
    {
        var changes = new FileChanges(simpleChanges.Split('\n'), "/tmp/canonLit_changes.txt");
        var list = changes.Find("section_replace.xml");

        Assert.True(list.Count == 1, "Cannot find section replace example.");
        
        string text = @"
The quick brown fox jumps over the lazy dog.
start with this
XXXXXXXXXXXXXXXXXXXXXXXXXX
this ends here
Sphinx of black quartz, judge my vow.
this ends here
        ".Trim();

        foreach(var change in list) {
            bool found = change.Apply(ref text);

            Assert.True(found, "Pattern did not match in section replace example.");
            Assert.True(text.Contains("YYYYYYYYYYYYYYYYYYYY"), "Pattern was not replaced in section replace example.");
            Assert.True(text.Contains("Sphinx"), "In section replace example the matching did not stop "
                + "at the first occurance of the end pattern.");
        }
    }
}
