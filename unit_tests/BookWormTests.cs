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

public class BookWormTests
{
    private static BookWorm<string> GetSampleObject() {
        var b = new BookWorm<string>();
        b.IncomingSection("book", "1");
        b.IncomingSection("section", "1");
        b.IncomingSection("paragraph", "1");
        b.AddElement("This is a sentence. 1.1.1.");
        b.IncomingSection("paragraph", "2");
        b.AddElement("This is a sentence. 1.1.2.");
        b.IncomingSection("paragraph", "3");
        b.AddElement("This is a sentence. 1.1.3.");
        b.IncomingSection("section", "2");
        b.IncomingSection("paragraph", "1");
        b.AddElement("This is a sentence. 1.2.1.");
        b.IncomingSection("paragraph", "2");
        b.AddElement("This is a sentence. 1.2.2.");
        b.IncomingSection("paragraph", "3");
        b.AddElement("This is a sentence. 1.2.3. A");
        b.AddElement("This is a sentence. 1.2.3. B");
        b.AddElement("This is a sentence. 1.2.3. C");
        b.IncomingSection("paragraph", "4");
        b.AddElement("This is a sentence. 1.2.4.");
        b.IncomingSection("book", "2");
        b.IncomingSection("section", "1");
        b.IncomingSection("paragraph", "1");
        b.AddElement("This is a sentence. 2.1.1.");

        return b;
    }

    [Fact]
    public void TestSectionKeys1()
    {
        var b = GetSampleObject();
        var keys = b.GetSectionKeyList();

        Assert.True(keys.Count == 8, "Invalid number of section keys found. Expected 8.");
        Assert.True(keys[5] == "book=1|section=2|paragraph=3", "As the fifth key expected: book=1|section=2|paragraph=3");

        var first = b.GetFirstNodeBySectionKey("book=1|section=2|paragraph=3");
        Assert.True(first != null, "Cannot find first node: 'This is a sentence. 1.2.3. A'.");
        Assert.True(first.Value == "This is a sentence. 1.2.3. A", $"First node has value '{first.Value}' "
            + " instead of 'This is a sentence. 1.2.3. A'.");

        var second = first.Next;
        Assert.True(second != null, "Cannot find second node in section: book=1|section=2|paragraph=3");
        Assert.True(second.Value == "This is a sentence. 1.2.3. B", $"Second node has value '{second.Value}' "
            + " instead of 'This is a sentence. 1.2.3. B'.");

        var third = second.Next;
        Assert.True(third != null, "Cannot find third node in section: book=1|section=2|paragraph=3");
        Assert.True(third.Value == "This is a sentence. 1.2.3. C", $"Second node has value '{third.Value}' "
            + " instead of 'This is a sentence. 1.2.3. C'.");

        var last = b.GetLastNodeBySectionKey("book=1|section=2|paragraph=3");
        Assert.True(last != null, "Cannot find last node in section: book=1|section=2|paragraph=3");
        Assert.True(last.Value == third.Value, "In section 'book=1|section=2|paragraph=3' the third "
            + $"and the last nodes are not the same. '{third.Value}' != '{last.Value}'");
    }

    [Fact]
    public void TestLastKey1() {
        var b = GetSampleObject();
        var first = b.GetFirstNodeBySectionKey("book=2|section=1|paragraph=1");

        Assert.True(first != null, "Cannot find first node in section book=2|section=1|paragraph=1.");
        Assert.True(first.Value == "This is a sentence. 2.1.1.", $"First node has value '{first.Value}' "
            + " instead of 'This is a sentence. 2.1.1.'.");
    }
}
