using System.Text;
using RainbowLatinReader;

namespace unit_tests;

public class XmlParserTests
{
    private readonly byte[] xmlData = Encoding.ASCII.GetBytes(@"
<root>
    <header>
        <title>The secret life of cats</title>
        <author>Dr. Meow Meowinsky</author>
        <author>Lone Katsengold</author>
    </header>

    <body>
        <section>
            <p>1 First sentence.</p>
            <p>1 Second sentence.</p>
            <p>1 Third sentence.</p>
        </section>

        <section>
            <p>2 First sentence.</p>
            <p>2 Second sentence.</p>
            <p>2 Third sentence.</p>
        </section>
    </body>
</root>
    ");

    [Fact]
    public void TestReadText1()
    {
        using var stream = new MemoryStream(xmlData);
        using XmlParser xml = new(stream, "/tmp/example.xml");
        bool found = xml.Next("body.section.p");

        Assert.True(found, "Wasn't able to find 'body.section.p'.");
        Assert.True(xml.GetContent() == "1 First sentence.",
            "Wasn't able to read text '1 First sentence.' from the "
            + "first 'body.section.p' node.");
    }

    [Fact]
    public void TestCaptures1()
    {
        using var stream = new MemoryStream(xmlData);
        using XmlParser xml = new(stream, "/tmp/example.xml");
        xml.SetTrap("root.header.title");
        xml.SetTrap("header.author");
        xml.Next("root.body");

        var captures = xml.GetCaptures();
        Assert.True(captures.ContainsKey("root.header.title"),
            "Did not find capture for trap 'root.header.title'.");
        Assert.True(captures.ContainsKey("header.author"),
            "Did not find capture for trap 'header.author'.");
        Assert.True(captures["root.header.title"][0] == "The secret life of cats",
            "The capatured title is invalid.");
        Assert.True(captures["header.author"].Count == 2,
            "Didn't find both authors.");

        xml.Next("body.section.p");
    }
}
