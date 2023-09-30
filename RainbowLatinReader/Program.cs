using System.Text;

using RainbowLatinReader;

/*
using var configFile = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), "config.ini"));
var config = new Config(configFile);
config.GetPerseusCanonicalLatinLitDir();
*/

MemoryStream stream = new(Encoding.ASCII.GetBytes(@"
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
"));

using RainbowLatinReader.XmlParser xml = new(stream, "/tmp/example.xml");
bool found = xml.Next("body.section.p");

