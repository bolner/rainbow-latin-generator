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

public class XmlParserTests
{
    private readonly byte[] xmlData1 = Encoding.ASCII.GetBytes(@"
<root>
    <header>
        <title>How to properly feed a cat</title>
        <author>Dr. Meow Meowinsky</author>
        <author>Leone Katzengold</author>
    </header>

    <body>
        <section number=""1.1.1"">
            <p>1 First sentence.</p>
            <p>1 Second sentence.</p>
            <p>1 Third sentence.</p>
        </section>

        <section number=""1.1.2"">
            <p>2 First sentence.</p>
            <p>2 Second sentence.</p>
            <p>2 Third sentence.</p>
        </section>
    </body>
</root>
    ");

    private readonly byte[] xmlData2 = Encoding.ASCII.GetBytes(@"
<root>
    <text>
        JULIUS CAESAR, the divine, <note anchored=""true""><foreign xml:lang=""lat"">Julius Caesar Divus</foreign>. Romulus, the founder of <placeName key=""perseus,Rome"">Rome</placeName>, had the honour of an apotheosis conferred on him by the senate, under the title of Quirinus, to obviate the people's suspicion of his having been taken off by a conspiracy of the patrician order. Political circumstances again concurred with popular superstition to revive this posthumous adulation in favour of Julius Caesar, the founder of the empire, who also fell by the hands of conspirators. It is remarkable in the history of a nation so jealous of public liberty, that, in both instances, they bestowed the highest mark of human homage upon men who owed their fate to the introduction of arbitrary power. </note> lost his father <note anchored=""true"">Pliny informs us that Caius Julius, the father of Julius Caesar, a man of praetorian rank, died suddenly at <placeName key=""perseus,Pisa"">Pisa</placeName>. </note> when he was in the sixteenth year of his age; <note anchored=""true"">A. U. C. (in the year from the foundation of <placeName key=""perseus,Rome"">Rome</placeName>) 670; A. C. (before Christ) about 92. </note> and the year following, being nominated to the office of high-priest of Jupiter, <note anchored=""true""><foreign xml:lang=""lat"">Flamen Dialis</foreign>. This was an office of great dignity, but subjected the holder to many restrictions. He was not allowed to ride on horseback, nor to absent himself from the city for a single night. His wife was also under particular restraints, and could not be divorced. If she died, the flamen resigned his office, because there were certain sacred rites which he could not perform without her assistance. Besides other marks of distinction, he wore a purple robe called laena, and a conical mitre called apex. </note> he repudiated Cossutia, who was very wealthy, although her family belonged only to the equestrian order, and to whom he had been contracted when he was a mere boy. He then married Cornelia, the daughter of Cinna, who was four times consul; and had by her, shortly afterwards, a daughter named Julia. Resisting all the efforts of the dictator Sylla to induce him to divorce Cornelia, he suffered the penalty of being stripped of his sacerdotal office, his wife's dowry, and his own patrimonial estates; and, being identified with the adverse faction, <note anchored=""true"">Two powerful parties were contending at <placeName key=""perseus,Rome"">Rome</placeName> for the supremacy; Sylla being at the head of the faction of the nobles, while Marius espoused the cause of the people. Sylla suspected Julius Caesar of belonging to the Marian party, because Marius had married his aunt Julia. </note> was compelled to withdraw from <placeName key=""perseus,Rome"">Rome</placeName>. After changing his place of concealment nearly every night, <note anchored=""true"">He wandered about for some time in the <placeName key=""tgn,7021127"">Sabine</placeName> territory. </note> although he was suffering from a quartan ague, and having effected his release by bribing the officers who had tracked his footsteps, he at length obtained a pardon through the intercession of the vestal virgins, and of Mamercus AEmilius and Aurelius Cotta, his near relatives. We are assured that when Sylla, having withstood for a while the entreaties of his own best friends, persons of distinguished rank, at last yielded to their importunity, he exclaimed-either by a divine impulse, or from a shrewd conjecture: ""Your suit is granted, and you may take him among you; but know,"" he added, "" that this man, for whose safety you are so extremely anxious, will, some day or other, be the ruin of the party of the nobles, in defence of which you are leagued with me; for in this one Caesar, you will find many a Marius.""
    </text>
</root>
    ");

    [Fact]
    public void TestNext1()
    {
        var canonFile = new MockCanonFile("/tmp/example.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlData1);
        using CanonLitXmlParser xml = new(canonFile, new List<string>{
            "body.section.p"
        });
        bool found = xml.Next();
        string text = xml.FetchContent() ?? "";

        Assert.True(found, "Wasn't able to find 'body.section.p'.");
        Assert.True(text == "1 First sentence.",
            "Wasn't able to read text '1 First sentence.' from the "
            + $"first 'body.section.p' node. Got '{text}' instead.");
    }

    [Fact]
    public void TestAttributes1()
    {
        var canonFile = new MockCanonFile("/tmp/example.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlData1);
        using CanonLitXmlParser xml = new(canonFile, new List<string>{
            "body.section"
        });

        xml.Next();
        var attributes = xml.GetAttributes();

        Assert.True(attributes.ContainsKey("number"), "Unable to find the 'number' attribute "
            + "on the first 'body.section' element.");

        Assert.True(attributes["number"] == "1.1.1", "The number attribute of the first "
            + "'body.section' element did not contain '1.1.1'.");
    }

    [Fact]
    public void TestTextWithElements1()
    {
        var canonFile = new MockCanonFile("/tmp/example.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlData2);
        using CanonLitXmlParser xml = new(canonFile, new List<string>{
            "root.text"
        });
        string expected = "JULIUS CAESAR, the divine, lost his father when he was in the sixteenth year of his age; and the year following, being nominated to the office of high-priest of Jupiter, he repudiated Cossutia, who was very wealthy, although her family belonged only to the equestrian order, and to whom he had been contracted when he was a mere boy. He then married Cornelia, the daughter of Cinna, who was four times consul; and had by her, shortly afterwards, a daughter named Julia. Resisting all the efforts of the dictator Sylla to induce him to divorce Cornelia, he suffered the penalty of being stripped of his sacerdotal office, his wife's dowry, and his own patrimonial estates; and, being identified with the adverse faction, was compelled to withdraw from Rome. After changing his place of concealment nearly every night, although he was suffering from a quartan ague, and having effected his release by bribing the officers who had tracked his footsteps, he at length obtained a pardon through the intercession of the vestal virgins, and of Mamercus AEmilius and Aurelius Cotta, his near relatives. We are assured that when Sylla, having withstood for a while the entreaties of his own best friends, persons of distinguished rank, at last yielded to their importunity, he exclaimed-either by a divine impulse, or from a shrewd conjecture: \"Your suit is granted, and you may take him among you; but know,\" he added, \" that this man, for whose safety you are so extremely anxious, will, some day or other, be the ruin of the party of the nobles, in defence of which you are leagued with me; for in this one Caesar, you will find many a Marius.\"";

        xml.Next();
        string text = xml.FetchContent() ?? "";

        Assert.True(text == expected,
            $"The extracted text is not what is expected.\nGot:\n{text}\n\n"
            + $"Expected:\n{expected}\n");
    }

    [Fact]
    public void TestChoiceStructureWithDel() {
        byte[] data = Encoding.ASCII.GetBytes(@"
<xml>
    <div n=""1"">
        patres nostri
        <choice>
            <sic>cumr</sic>
            <corr>cum</corr>
        </choice>
        in Hispania Carthaginiensium et imperatores
    </div>

    <div n=""2"">
        <choice>
            <sic>ibij</sic>
            <corr><del>ibi</del></corr>
        </choice>
        et exercitus essent,
        <choice>
            <sic>ippsi</sic>
            <corr>ipsi</corr>
        </choice>
        nullum
    </div>
</xml>
        ");

        var canonFile = new MockCanonFile("/tmp/example.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, data);
        using CanonLitXmlParser xml = new(canonFile, [ "xml.div" ]);
        
        xml.Next();
        bool found = xml.Next();

        Assert.True(found, "Wasn't able to find the second 'xml.div'.");
        xml.Next();

        string text = (xml.FetchTextBuffer() ?? "").Trim();
        string expected = "et exercitus essent, ipsi nullum";

        Assert.True(text == expected,
            $"The extracted text is not what is expected.\nGot:\n{text}\n\n"
            + $"Expected:\n{expected}\n");
    }
}
