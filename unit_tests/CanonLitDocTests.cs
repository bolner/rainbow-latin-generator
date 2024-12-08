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

public class CanonLitDocTests
{
    private readonly byte[] xmlDataLatin = Encoding.ASCII.GetBytes(@"
<TEI xmlns=""http://www.tei-c.org/ns/1.0"">
  <teiHeader xml:lang=""eng"">
    <fileDesc>
      <titleStmt>
        <title xml:lang=""lat"">Divus Julius</title>
        <author>Suetonius</author>
        <editor>Maximilian Ihm</editor>
        <sponsor>Perseus Project, Tufts University</sponsor>
        <principal>Gregory Crane</principal>
        <respStmt>
          <resp>Prepared under the supervision of</resp>
          <name>Bridget Almas</name>
          <name>Lisa Cerrato</name>
          <name>William Merrill</name>
          <name>David Smith</name>
        </respStmt>
        <funder n=""org:FIPSE"">Fund for the Improvement of Postsecondary Education</funder> 
      </titleStmt>
      <publicationStmt>
        <publisher>Trustees of Tufts University</publisher>
        <pubPlace>Medford, MA</pubPlace>
        <authority>Perseus Project</authority>
        <date type=""release"">2000-08-01</date>
      </publicationStmt>
    </fileDesc>
  </teiHeader>

  <text>
    <body>
      <div n=""urn:cts:latinLit:phi1348.abo011.perseus-lat2"" type=""edition"" xml:lang=""lat"">
      <div n=""1"" type=""textpart"" subtype=""chapter""><div n=""1"" type=""textpart"" subtype=""section"">
        <p>* * *</p>
        <p>Annum agens sextum decimum patrem amisit; sequentibusque consulibus flamen Dialis
          destinatus dimissa Cossutia, quae familia equestri sed admodum diues praetextato
          desponsata fuerat, Corneliam Cinnae quater consulis filiam duxit uxorem, ex qua illi mox
          Iulia nata est; neque ut repudiaret compelli a <persName key=""Sulla"">dictatore
            Sulla</persName> ullo modo potuit. </p></div><div type=""textpart"" subtype=""section"" n=""2""><p rend=""merged""> quare et sacerdotio
          et uxoris dote et gentilicis hereditatibus multatus diuersarum partium habebatur, ut etiam
          discedere e medio et quamquam morbo quartanae adgrauante prope per singulas noctes
          commutare latebras cogeretur seque ab inquisitoribus pecunia redimeret, donec per uirgines
          Vestales perque Mamercum Aemilium et Aurelium Cottam propinquos et adfines suos ueniam
          impetrauit. </p></div><div type=""textpart"" subtype=""section"" n=""3""><p rend=""merged""> satis constat Sullam, cum deprecantibus
          amicissimis et ornatissimis uiris aliquamdiu denegasset atque illi pertinaciter
          contenderent, expugnatum tandem proclamasse siue diuinitus siue aliqua coniectura:
            <quote>uincerent ac sibi haberent, dum modo scirent eum, quem incolumem tanto opere
            cuperent, quandoque optimatium partibus, quas secum simul defendissent, exitio futurum;
            nam Caesari multos Marios inesse.</quote></p>
      </div></div>
      <div n=""2"" type=""textpart"" subtype=""chapter""><div n=""1"" type=""textpart"" subtype=""section"">
        <p>Stipendia prima in Asia fecit Marci Thermi praetoris contubernio; a quo ad accersendam
          classem in Bithyniam missus desedit apud Nicomeden, non sine rumore prostratae regi
          pudicitiae; quem rumorem auxit intra paucos rursus dies repetita <placeName key=""Bithynia"">Bithynia</placeName> per causam exigendae pecuniae, quae deberetur cuidam libertino
          clienti suo. reliqua militia secundiore fama fuit et a Thermo in expugnatione Mytilenarum
          corona ciuica donatus est.</p>
      </div></div>

	  <div n=""4"" type=""textpart"" subtype=""chapter"">
	  <p>This is additional chapter no 4, which is also present in the latin doc.</p>
	  </div>

	  </div>
    </body>
  </text>
</TEI>
    ");

    private readonly byte[] xmlDataEnglish = Encoding.ASCII.GetBytes(@"
<TEI xmlns=""http://www.tei-c.org/ns/1.0"">
	<teiHeader xml:lang=""eng"">
		<fileDesc>
			<titleStmt>
				<title>Julius Caesar</title>
				<author>Suetonius</author>
				<editor role=""translator"">Alexander Thomson</editor>
				<sponsor>Perseus Project, Tufts University</sponsor>
				<principal>Gregory Crane</principal>
				<respStmt>
					<resp>Prepared under the supervision of</resp>
					<name>Bridget Almas</name>
					<name>Lisa Cerrato</name>
					<name>William Merrill</name>
					<name>David Smith</name>
				</respStmt>
				<funder n=""org:FIPSE"">Fund for the Improvement of Postsecondary Education</funder>
			</titleStmt>
			<publicationStmt>
				<publisher>Trustees of Tufts University</publisher>
				<pubPlace>Medford, MA</pubPlace>
				<authority>Perseus Project</authority>
				<date type=""release"" cert=""unknown"">2000-08-01</date>
			</publicationStmt>
        </fileDesc>
    </teiHeader>

    <text xml:lang=""eng"">
		<body><div xml:lang=""eng"" type=""translation"" n=""urn:cts:latinLit:phi1348.abo011.perseus-eng2"">
			<div type=""textpart"" n=""1"" subtype=""chapter"">
				<p>JULIUS CAESAR, the divine, <note anchored=""true"">
						<foreign xml:lang=""lat"">Julius Caesar Divus</foreign>. Romulus, the founder
						of <placeName key=""perseus,Rome"">Rome</placeName>, had the honour of an
						apotheosis conferred on him by the senate, under the title of Quirinus, to
						obviate the people's suspicion of his having been taken off by a conspiracy
						of the patrician order. Political circumstances again concurred with popular
						superstition to revive this posthumous adulation in favour of Julius Caesar,
						the founder of the empire, who also fell by the hands of conspirators. It is
						remarkable in the history of a nation so jealous of public liberty, that, in
						both instances, they bestowed the highest mark of human homage upon men who
						owed their fate to the introduction of arbitrary power. </note> lost his
					father <note anchored=""true"">Pliny informs us that Caius Julius, the father of
						Julius Caesar, a man of praetorian rank, died suddenly at <placeName key=""perseus,Pisa"">Pisa</placeName>. </note> when he was in the
					sixteenth year of his age; <note anchored=""true"">A. U. C. (in the year from the
						foundation of <placeName key=""perseus,Rome"">Rome</placeName>) 670; A. C.
						(before Christ) about 92. </note> and the year following, being nominated to
					the office of high-priest of Jupiter,
						<note anchored=""true"">
						<foreign xml:lang=""lat"">Flamen Dialis</foreign>. This was an office of great
						dignity, but subjected the holder to many restrictions. He was not allowed
						to ride on horseback, nor to absent himself from the city for a single
						night. His wife was also under particular restraints, and could not be
						divorced. If she died, the flamen resigned his office, because there were
						certain sacred rites which he could not perform without her assistance.
						Besides other marks of distinction, he wore a purple robe called laena, and
						a conical mitre called apex. </note> he repudiated Cossutia, who was very
					wealthy, although her family belonged only to the equestrian order, and to whom
					he had been contracted when he was a mere boy. He then married Cornelia, the
					daughter of Cinna, who was four times consul; and had by her, shortly
					afterwards, a daughter named Julia. Resisting all the efforts of the dictator
					Sylla to induce him to divorce Cornelia, he suffered the penalty of being
					stripped of his sacerdotal office, his wife's dowry, and his own patrimonial
					estates; and, being identified with the adverse faction, <note anchored=""true"">Two powerful parties were contending at <placeName key=""perseus,Rome"">Rome</placeName> for the supremacy; Sylla being at the head of the
						faction of the nobles, while Marius espoused the cause of the people. Sylla
						suspected Julius Caesar of belonging to the Marian party, because Marius had
						married his aunt Julia. </note> was compelled to withdraw from <placeName key=""perseus,Rome"">Rome</placeName>. After changing his place of concealment
					nearly every night, <note anchored=""true"">He wandered about for some time in the
							<placeName key=""tgn,7021127"">Sabine</placeName> territory. </note>
					although he was suffering from a quartan ague, and having effected his release
					by bribing the officers who had tracked his footsteps, he at length obtained a
					pardon through the intercession of the vestal virgins, and of Mamercus AEmilius
					and Aurelius Cotta, his near relatives.
					
					This is an <abbr><expan>abbreviation</expan>abbr.</abbr> example.
					This is another <abbr>abbr.<expan>abbreviation</expan></abbr> example.
					
					We are assured that when Sylla, having
					withstood for a while the entreaties of his own best friends, persons of
					distinguished rank, at last yielded to their importunity, he exclaimed-either by
					a divine impulse, or from a shrewd conjecture: ""Your suit is granted, and you
					may take him among you; but know,"" he added, "" that this man, for whose safety
					you are so extremely anxious, will, some day or other, be the ruin of the party
					of the nobles, in defence of which you are leagued with me; for in this one
					Caesar, you will find many a Marius.""</p>
			</div>
			<div type=""textpart"" n=""2"" subtype=""chapter"">
				<p>His first campaign was served in <placeName key=""tgn,1000004"">Asia</placeName>,
					on the staff of the praetor, M. Thermus; and being dispatched into <placeName key=""tgn,7016608"">Bithynia</placeName>, <note anchored=""true""><placeName key=""tgn,7016608"">Bithynia</placeName>, in <placeName key=""tgn,7002294"">Asia Minor</placeName>, was bounded on the south by <placeName key=""tgn,7002613"">Phrygia</placeName>; on the west by the Bosphorus and
						Propontis; and on the north by the Euxine sea. Its boundaries towards the
						east are not clearly ascertained, Strabo, Pliny, and Ptolemy differing from
						each other on the subject. </note> to bring thence a fleet, he loitered so
					long at the court of Nicomedes, as to give occasion to reports of lewd
					proceedings between him and that prince; which received additional credit from
					his hasty return to <placeName key=""tgn,7016608"">Bithynia</placeName>, under the
					pretext of recovering a debt due to a freedman, his client. The rest of his
					service was more favourable to his reputation; and when <placeName key=""tgn,7002672"">Mitylene</placeName>
					<note anchored=""true""><placeName key=""tgn,7002672"">Mitylene</placeName> was a
						city in the island of <placeName key=""tgn,7002672"">Lesbos</placeName>,
						famous for the study of philosophy and eloquence. According to Pliny, it
						remained a free city and in power one thousand five hundred years. It
						suffered much in the Peloponnesian war from the Athenians, and in the
						Mithridatic from the Romans, by whom it was taken and destroyed. But it soon
						rose again, having recovered its ancient liberty by the favour of Pompey;
						and was afterwards much embellished by Trajan, who added to it the splendour
						of his own name. This was the country of Pittacus, one of the seven wise men
						of <placeName key=""tgn,1000074"">Greece</placeName>, as well as of Alcaeus
						and Sappho. The natives showed a particular taste for poetry, and had, as
						Plutarch informs us, stated times for the celebration of poetical contests.
					</note> was taken by storm, he was presented by Thermus with the civic crown.
						<note anchored=""true"">The civic crown was made of oak-leaves, and given to
						him who had saved the life of a citizen. The person thus decorated wore it
						at public spectacles, and sat next the senators. When he entered, the
						audience rose up, as a mark of respect. </note></p>
			</div></div>
        </body>
    </text>
</TEI>
    ");

	private readonly byte[] xmlDataEnglishWithExtra = Encoding.ASCII.GetBytes(@"
<TEI xmlns=""http://www.tei-c.org/ns/1.0"">
	<teiHeader xml:lang=""eng"">
		<fileDesc>
			<titleStmt>
				<title>Julius Caesar</title>
				<author>Suetonius</author>
				<editor role=""translator"">Alexander Thomson</editor>
				<sponsor>Perseus Project, Tufts University</sponsor>
				<principal>Gregory Crane</principal>
				<respStmt>
					<resp>Prepared under the supervision of</resp>
					<name>Bridget Almas</name>
					<name>Lisa Cerrato</name>
					<name>William Merrill</name>
					<name>David Smith</name>
				</respStmt>
				<funder n=""org:FIPSE"">Fund for the Improvement of Postsecondary Education</funder>
			</titleStmt>
			<publicationStmt>
				<publisher>Trustees of Tufts University</publisher>
				<pubPlace>Medford, MA</pubPlace>
				<authority>Perseus Project</authority>
				<date type=""release"" cert=""unknown"">2000-08-01</date>
			</publicationStmt>
        </fileDesc>
    </teiHeader>

    <text xml:lang=""eng"">
		<body><div xml:lang=""eng"" type=""translation"" n=""urn:cts:latinLit:phi1348.abo011.perseus-eng2"">
			<div type=""textpart"" n=""1"" subtype=""chapter"">
				<p>JULIUS CAESAR, the divine, <note anchored=""true"">
						<foreign xml:lang=""lat"">Julius Caesar Divus</foreign>. Romulus, the founder
						of <placeName key=""perseus,Rome"">Rome</placeName>, had the honour of an
						apotheosis conferred on him by the senate, under the title of Quirinus, to
						obviate the people's suspicion of his having been taken off by a conspiracy
						of the patrician order. Political circumstances again concurred with popular
						superstition to revive this posthumous adulation in favour of Julius Caesar,
						the founder of the empire, who also fell by the hands of conspirators. It is
						remarkable in the history of a nation so jealous of public liberty, that, in
						both instances, they bestowed the highest mark of human homage upon men who
						owed their fate to the introduction of arbitrary power. </note> lost his
					father <note anchored=""true"">Pliny informs us that Caius Julius, the father of
						Julius Caesar, a man of praetorian rank, died suddenly at <placeName key=""perseus,Pisa"">Pisa</placeName>. </note> when he was in the
					sixteenth year of his age; <note anchored=""true"">A. U. C. (in the year from the
						foundation of <placeName key=""perseus,Rome"">Rome</placeName>) 670; A. C.
						(before Christ) about 92. </note> and the year following, being nominated to
					the office of high-priest of Jupiter,
						<note anchored=""true"">
						<foreign xml:lang=""lat"">Flamen Dialis</foreign>. This was an office of great
						dignity, but subjected the holder to many restrictions. He was not allowed
						to ride on horseback, nor to absent himself from the city for a single
						night. His wife was also under particular restraints, and could not be
						divorced. If she died, the flamen resigned his office, because there were
						certain sacred rites which he could not perform without her assistance.
						Besides other marks of distinction, he wore a purple robe called laena, and
						a conical mitre called apex. </note> he repudiated Cossutia, who was very
					wealthy, although her family belonged only to the equestrian order, and to whom
					he had been contracted when he was a mere boy. He then married Cornelia, the
					daughter of Cinna, who was four times consul; and had by her, shortly
					afterwards, a daughter named Julia. Resisting all the efforts of the dictator
					Sylla to induce him to divorce Cornelia, he suffered the penalty of being
					stripped of his sacerdotal office, his wife's dowry, and his own patrimonial
					estates; and, being identified with the adverse faction, <note anchored=""true"">Two powerful parties were contending at <placeName key=""perseus,Rome"">Rome</placeName> for the supremacy; Sylla being at the head of the
						faction of the nobles, while Marius espoused the cause of the people. Sylla
						suspected Julius Caesar of belonging to the Marian party, because Marius had
						married his aunt Julia. </note> was compelled to withdraw from <placeName key=""perseus,Rome"">Rome</placeName>. After changing his place of concealment
					nearly every night, <note anchored=""true"">He wandered about for some time in the
							<placeName key=""tgn,7021127"">Sabine</placeName> territory. </note>
					although he was suffering from a quartan ague, and having effected his release
					by bribing the officers who had tracked his footsteps, he at length obtained a
					pardon through the intercession of the vestal virgins, and of Mamercus AEmilius
					and Aurelius Cotta, his near relatives. We are assured that when Sylla, having
					withstood for a while the entreaties of his own best friends, persons of
					distinguished rank, at last yielded to their importunity, he exclaimed-either by
					a divine impulse, or from a shrewd conjecture: ""Your suit is granted, and you
					may take him among you; but know,"" he added, "" that this man, for whose safety
					you are so extremely anxious, will, some day or other, be the ruin of the party
					of the nobles, in defence of which you are leagued with me; for in this one
					Caesar, you will find many a Marius.""</p>
			</div>
			<div type=""textpart"" n=""2"" subtype=""chapter"">
				<p>His first campaign was served in <placeName key=""tgn,1000004"">Asia</placeName>,
					on the staff of the praetor, M. Thermus; and being dispatched into <placeName key=""tgn,7016608"">Bithynia</placeName>, <note anchored=""true""><placeName key=""tgn,7016608"">Bithynia</placeName>, in <placeName key=""tgn,7002294"">Asia Minor</placeName>, was bounded on the south by <placeName key=""tgn,7002613"">Phrygia</placeName>; on the west by the Bosphorus and
						Propontis; and on the north by the Euxine sea. Its boundaries towards the
						east are not clearly ascertained, Strabo, Pliny, and Ptolemy differing from
						each other on the subject. </note> to bring thence a fleet, he loitered so
					long at the court of Nicomedes, as to give occasion to reports of lewd
					proceedings between him and that prince; which received additional credit from
					his hasty return to <placeName key=""tgn,7016608"">Bithynia</placeName>, under the
					pretext of recovering a debt due to a freedman, his client. The rest of his
					service was more favourable to his reputation; and when <placeName key=""tgn,7002672"">Mitylene</placeName>
					<note anchored=""true""><placeName key=""tgn,7002672"">Mitylene</placeName> was a
						city in the island of <placeName key=""tgn,7002672"">Lesbos</placeName>,
						famous for the study of philosophy and eloquence. According to Pliny, it
						remained a free city and in power one thousand five hundred years. It
						suffered much in the Peloponnesian war from the Athenians, and in the
						Mithridatic from the Romans, by whom it was taken and destroyed. But it soon
						rose again, having recovered its ancient liberty by the favour of Pompey;
						and was afterwards much embellished by Trajan, who added to it the splendour
						of his own name. This was the country of Pittacus, one of the seven wise men
						of <placeName key=""tgn,1000074"">Greece</placeName>, as well as of Alcaeus
						and Sappho. The natives showed a particular taste for poetry, and had, as
						Plutarch informs us, stated times for the celebration of poetical contests.
					</note> was taken by storm, he was presented by Thermus with the civic crown.
						<note anchored=""true"">The civic crown was made of oak-leaves, and given to
						him who had saved the life of a citizen. The person thus decorated wore it
						at public spectacles, and sat next the senators. When he entered, the
						audience rose up, as a mark of respect. </note></p>
			</div>

			<div type=""textpart"" n=""3"" subtype=""chapter"">
			<p>This is additional chapter no 3, which is not present in the latin doc.</p>
			</div>

			<div type=""textpart"" n=""4"" subtype=""chapter"">
			<p>This is additional chapter no 4, which is also present in the latin doc.</p>
			</div>

			<div type=""textpart"" n=""5"" subtype=""chapter"">
			<p>This is additional chapter no 5, which is not present in the latin doc.</p>
			</div>

			</div>
        </body>
    </text>
</TEI>
    ");

	private readonly byte[] canonLitChangesXML = Encoding.ASCII.GetBytes(@"
<changes>
    <add
        documentID=""phi1348.abo011""
        language=""english""
        after=""chapter=2""
        key=""chapter=2b""
        before=""chapter=3""
    >
    This chapter is coming from the changes XML.
    </add>
</changes>
    ");

    [Fact]
    public void TestMetaFields1()
    {
        var latinFile = new MockCanonFile("/tmp/example_latin.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlDataLatin);
        var englishFile = new MockCanonFile("/tmp/example_english.xml", "phi1348.abo011",
            ICanonFile.Language.English, 2, xmlDataEnglish);
        var canonParserFactory = new XmlParserFactory();
		var canonLitChanges =  new CanonLitChanges(new MemoryStream(canonLitChangesXML));
        
        var doc = new CanonLitDoc(latinFile, englishFile, canonParserFactory, new BookWorm<string>(),
			new BookWorm<string>(), canonLitChanges, new MockLogging());
		doc.Process();

		Assert.True(doc.GetLastError() == null, "Document processing failed with error: "
			+ (doc.GetLastError() ?? new RainbowLatinException("?")).ToString());

        Assert.True(doc.GetDocumentID() == "phi1348.abo011", "No valid document ID found.");
        Assert.True(doc.GetEnglishTitle() == "Julius Caesar", "No valid title found in English document.");
        Assert.True(doc.GetLatinAuthor() == "Suetonius", "No valid author found in Latin document.");
    }

	[Fact]
    public void TestSections1()
    {
        var latinFile = new MockCanonFile("/tmp/example_latin.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlDataLatin);
        var englishFile = new MockCanonFile("/tmp/example_english.xml", "phi1348.abo011",
            ICanonFile.Language.English, 2, xmlDataEnglish);
        var canonParserFactory = new XmlParserFactory();
		var canonLitChanges =  new CanonLitChanges(new MemoryStream(canonLitChangesXML));
        
		var latin = new BookWorm<string>();
		var english = new BookWorm<string>();
        var doc = new CanonLitDoc(latinFile, englishFile, canonParserFactory, latin,
			english, canonLitChanges, new MockLogging());
		doc.Process();

		Assert.True(doc.GetLastError() == null, "Document processing failed with error: "
			+ (doc.GetLastError() ?? new RainbowLatinException("?")).ToString());

		var s1 = doc.GetEnglishSection("chapter=2");
        Assert.True(s1 != null, "Chapter '2' not found");

		// 
		s1 = doc.GetEnglishSection("chapter=2b");
        Assert.True(s1 != null, "Chapter '2b' not found");
		Assert.True(s1 == "This chapter is coming from the changes XML.", 
			$"Chapter '2b' contains '{s1}' instead of 'This chapter is coming from the changes XML.'.");
    }

	[Fact]
    public void TestExtraSections1()
    {
        var latinFile = new MockCanonFile("/tmp/example_latin.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlDataLatin);
        var englishFile = new MockCanonFile("/tmp/example_english.xml", "phi1348.abo011",
            ICanonFile.Language.English, 2, xmlDataEnglishWithExtra);
        var canonParserFactory = new XmlParserFactory();
		var canonLitChanges =  new CanonLitChanges(new MemoryStream(canonLitChangesXML));
        
		var latin = new BookWorm<string>();
		var english = new BookWorm<string>();
        var doc = new CanonLitDoc(latinFile, englishFile, canonParserFactory, latin,
			english, canonLitChanges, new MockLogging());
		doc.Process();

		Assert.True(doc.GetLastError() == null, "Document processing failed with error: "
			+ (doc.GetLastError() ?? new RainbowLatinException("?")).ToString());

		string? sec = doc.GetEnglishSection("chapter=4");
        Assert.True(sec != null, "Chapter '4' not found");
		Assert.True(sec == "This is additional chapter no 4, which is also present in the latin doc.",
			$"Chapter '4' contains '{sec}' instead of 'This is additional "
			+ "chapter no 4, which is also present in the latin doc.'.");
    }
}
