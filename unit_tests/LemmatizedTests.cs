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

public class LemmatizedTests
{
    private readonly byte[] xmlData = Encoding.ASCII.GetBytes(@"
<TEI xmlns=""http://www.tei-c.org/ns/1.0"">
    <teiHeader n=""urn:cts:latinLit:phi1348.abo011.perseus-lat2"">
    
        <fileDesc>
            <titleStmt>
                <title>Divus Julius</title>
                <author>Suetonius ca. 69-ca. 122</author>
            </titleStmt>
            <publicationStmt>
                <publisher>
                    <persName>Thibault Clérice</persName>
                </publisher>
            </publicationStmt>
            <sourceDesc>
                <bibl>
                    <idno>urn:cts:latinLit:phi1348.abo011.perseus-lat2</idno>
                    <link target=""https://github.com/PerseusDL/canonical-latinLit/archive/0.0.767.zip""/>
                    <dim source=""xml"" type=""md5-checksum"">98c90a59e562eeb254e5456a3ad63e9e</dim>
                    <dim source=""plaintext-transformation"" type=""md5-checksum"">50640278264dc7f5c77fe1e311f3ac26</dim>
                    <dim source=""xsl"" type=""md5-checksum"">22f0c529b7ecf86f9fc04cc8de63d204</dim>
                </bibl>
            </sourceDesc>
        </fileDesc>
    </teiHeader>
    <text n=""urn:cts:latinLit:phi1348.abo011.perseus-lat2"">
        <body>
            
        <ab type=""unknown"" n=""urn:cts:latinLit:phi1348.abo011.perseus-lat2:1"">
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""*"">*</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""*"">*</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""*"">*</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""annus"">Annum</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Nom|Numb=Sing|Gend=Com|Mood=Par|Tense=Pres|Voice=Act"" lemma=""ago"">agens</w>
                <w rend=""unknown"" n=""1"" pos=""ADJord"" msd=""Case=Acc|Numb=Sing|Gend=MascNeut|Deg=Pos"" lemma=""sextus"">sextum</w>
                <w rend=""unknown"" n=""1"" pos=""ADJord"" msd=""Case=Acc|Numb=Sing|Gend=MascNeut|Deg=Pos"" lemma=""decimus"">decimum</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""pater"">patrem</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""amitto"">amisit</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="";"">;</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Abl|Numb=Plur|Gend=Com|Deg=Pos|Mood=Par|Tense=Pres|Voice=Dep"" lemma=""sequor"">sequentibusque</w>
                <w rend=""unknown"" n=""1"" pos=""CON"" msd=""MORPH=empty"" lemma=""que"">{sequentibusque}</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Plur"" lemma=""consul"">consulibus</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""flamen1"">flamen</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Nom|Numb=Sing|Deg=Pos"" lemma=""Dialis"">Dialis</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Gend=Masc|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""destino"">destinatus</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Abl|Numb=Plur|Gend=Fem|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""dimitto"">dimissa</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Abl|Numb=Plur"" lemma=""Cossutia"">Cossutia</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""PROrel"" msd=""Case=Nom|Numb=Sing|Gend=Fem"" lemma=""qui1"">quae</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""familia"">familia</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Abl|Numb=Sing|Gend=Com|Deg=Pos"" lemma=""equester"">equestri</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""sed"">sed</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""admodum"">admodum</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Nom|Numb=Sing|Gend=Com|Deg=Pos"" lemma=""diues"">diues</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Abl|Numb=Sing|Gend=MascNeut|Deg=Pos"" lemma=""praetextatus"">praetextato</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Nom|Numb=Sing|Gend=Fem|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""desponso"">desponsata</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Pqp|Voice=Act|Person=3"" lemma=""sum1"">fuerat</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Cornelia"">Corneliam</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Gen|Numb=Sing"" lemma=""Cinna"">Cinnae</w>
                <w rend=""unknown"" n=""1"" pos=""ADJadv.mul"" msd=""MORPH=empty"" lemma=""quater"">quater</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Gen|Numb=Sing"" lemma=""consul"">consulis</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""filia"">filiam</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""duco"">duxit</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""uxor"">uxorem</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""ex"">ex</w>
                <w rend=""unknown"" n=""1"" pos=""PROrel"" msd=""Case=Abl|Numb=Sing|Gend=Fem"" lemma=""qui1"">qua</w>
                <w rend=""unknown"" n=""1"" pos=""PROdem"" msd=""Case=Dat|Numb=Sing|Gend=Com"" lemma=""ille"">illi</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""mox"">mox</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Nom|Numb=Sing|Gend=Fem"" lemma=""Iulius"">Iulia</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Nom|Numb=Sing|Gend=Fem|Mood=Par|Tense=Perf|Voice=Dep"" lemma=""nascor"">nata</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Pres|Voice=Act|Person=3"" lemma=""sum1"">est</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="";"">;</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""neque"">neque</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""MORPH=empty"" lemma=""ut4"">ut</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Sub|Tense=Impa|Voice=Act|Person=3"" lemma=""repudio"">repudiaret</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Mood=Inf|Tense=Pres|Voice=Pass"" lemma=""compello2"">compelli</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""ab"">a</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""dictator"">dictatore</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Nom|Numb=Sing"" lemma=""Sylla"">Sulla</w>
                <w rend=""unknown"" n=""1"" pos=""PROind"" msd=""Case=Abl|Numb=Sing|Gend=MascNeut"" lemma=""ullus"">ullo</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""modus"">modo</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""possum1"">potuit</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""."">.</w>
                <w rend=""unknown"" n=""1"" pos=""ADVrel"" msd=""MORPH=empty"" lemma=""quare2"">quare</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""sacerdotium"">sacerdotio</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Gen|Numb=Sing"" lemma=""uxor"">uxoris</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""dos"">dote</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Abl|Numb=Plur|Gend=Com|Deg=Pos"" lemma=""gentilicus"">gentilicis</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Plur"" lemma=""hereditas"">hereditatibus</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Nom|Numb=Sing|Gend=Masc|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""multo1"">multatus</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Gen|Numb=Plur|Gend=Fem|Deg=Pos"" lemma=""diuersus"">diuersarum</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Gen|Numb=Plur"" lemma=""pars"">partium</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Impa|Voice=Pass|Person=3"" lemma=""habeo"">habebatur</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""MORPH=empty"" lemma=""ut4"">ut</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""etiam"">etiam</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Mood=Inf|Tense=Pres|Voice=Act"" lemma=""discedo1"">discedere</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""ex"">e</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""medium"">medio</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""MORPH=empty"" lemma=""quamquam2"">quamquam</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""morbus"">morbo</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Gen|Numb=Sing"" lemma=""quartana"">quartanae</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Abl|Numb=Sing|Gend=Com|Mood=Par|Tense=Pres|Voice=Act"" lemma=""aggrauo"">adgrauante</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""prope1"">prope</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""per"">per</w>
                <w rend=""unknown"" n=""1"" pos=""ADJdis"" msd=""Case=Acc|Numb=Plur|Gend=Fem"" lemma=""singulus"">singulas</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Plur"" lemma=""nox"">noctes</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Mood=Inf|Tense=Pres|Voice=Act"" lemma=""commuto2"">commutare</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Plur"" lemma=""latebra"">latebras</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Sub|Tense=Impa|Voice=Pass|Person=3"" lemma=""cogo"">cogeretur</w>
                <w rend=""unknown"" n=""1"" pos=""PROref"" msd=""Case=Acc|Numb=Sing"" lemma=""sui"">seque</w>
                <w rend=""unknown"" n=""1"" pos=""CON"" msd=""MORPH=empty"" lemma=""que"">{seque}</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""ab"">ab</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Plur"" lemma=""inquisitor"">inquisitoribus</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Nom|Numb=Sing"" lemma=""pecunia"">pecunia</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Sub|Tense=Impa|Voice=Act|Person=3"" lemma=""redimo"">redimeret</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""MORPH=empty"" lemma=""donec"">donec</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""per"">per</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Plur"" lemma=""uirgo"">uirgines</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Acc|Numb=Plur|Gend=MascFem"" lemma=""Uestalis"">Vestales</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""per"">perque</w>
                <w rend=""unknown"" n=""1"" pos=""CON"" msd=""MORPH=empty"" lemma=""que"">{perque}</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Mamercus"">Mamercum</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Aemilius"">Aemilium</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Aurelius"">Aurelium</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Cotta"">Cottam</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Plur|Gend=Masc"" lemma=""propinqui"">propinquos</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Plur"" lemma=""affinis1"">adfines</w>
                <w rend=""unknown"" n=""1"" pos=""PROpos.ref"" msd=""Case=Acc|Numb=Plur|Gend=Masc"" lemma=""suus"">suos</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""uenia"">ueniam</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""impetro"">impetrauit</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""."">.</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""satis2"">satis</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Pres|Voice=Act|Person=3"" lemma=""consto"">constat</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Sylla"">Sullam</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""PRE"" msd=""MORPH=empty"" lemma=""cum2"">cum</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Abl|Numb=Plur|Gend=Com|Mood=Par|Tense=Pres|Voice=Dep"" lemma=""deprecor"">deprecantibus</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Abl|Numb=Plur|Gend=Com|Deg=Sup"" lemma=""amicus2"">amicissimis</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Abl|Numb=Plur|Gend=Com|Deg=Sup"" lemma=""ornatus2"">ornatissimis</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Plur"" lemma=""uir"">uiris</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""aliquamdiu"">aliquamdiu</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Sing|Mood=Sub|Tense=Pqp|Voice=Act|Person=3"" lemma=""denego"">denegasset</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""atque1"">atque</w>
                <w rend=""unknown"" n=""1"" pos=""PROdem"" msd=""Case=Nom|Numb=Plur|Gend=Masc"" lemma=""ille"">illi</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""pertinaciter"">pertinaciter</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Plur|Mood=Sub|Tense=Impa|Voice=Act|Person=3"" lemma=""contendo"">contenderent</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Acc|Numb=Sing|Gend=MascNeut|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""expugno"">expugnatum</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""tandem"">tandem</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Mood=Inf|Tense=Perf|Voice=Act"" lemma=""proclamo"">proclamasse</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""MORPH=empty"" lemma=""siue2"">siue</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""diuinitus"">diuinitus</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""MORPH=empty"" lemma=""siue2"">siue</w>
                <w rend=""unknown"" n=""1"" pos=""PROind"" msd=""Case=Abl|Numb=Sing|Gend=Fem"" lemma=""aliquis"">aliqua</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""coniectura"">coniectura</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="":"">:</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Plur|Mood=Sub|Tense=Impa|Voice=Act|Person=3"" lemma=""uinco"">uincerent</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""ac1"">ac</w>
                <w rend=""unknown"" n=""1"" pos=""PROref"" msd=""Case=Dat|Numb=Sing"" lemma=""sui1"">sibi</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Plur|Mood=Sub|Tense=Impa|Voice=Act|Person=3"" lemma=""habeo"">haberent</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""MORPH=empty"" lemma=""dum2"">dum</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""modo1"">modo</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Plur|Mood=Sub|Tense=Impa|Voice=Act|Person=3"" lemma=""scio"">scirent</w>
                <w rend=""unknown"" n=""1"" pos=""PROdem"" msd=""Case=Acc|Numb=Sing|Gend=Masc"" lemma=""is"">eum</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""PROrel"" msd=""Case=Acc|Numb=Sing|Gend=Masc"" lemma=""qui1"">quem</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Acc|Numb=Sing|Gend=MascFem|Deg=Pos"" lemma=""incolumis"">incolumem</w>
                <w rend=""unknown"" n=""1"" pos=""PROdem"" msd=""Case=Abl|Numb=Sing|Gend=MascNeut"" lemma=""tantus"">tanto</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""opus1"">opere</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Plur|Mood=Sub|Tense=Impa|Voice=Act|Person=3"" lemma=""cupio"">cuperent</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""CONsub"" msd=""Deg=Pos"" lemma=""quandoque2"">quandoque</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Gen|Numb=Plur"" lemma=""optimates"">optimatium</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Dat|Numb=Plur"" lemma=""pars"">partibus</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""PROrel"" msd=""Case=Acc|Numb=Plur|Gend=Fem"" lemma=""qui1"">quas</w>
                <w rend=""unknown"" n=""1"" pos=""PROref"" msd=""Case=Abl|Numb=Sing"" lemma=""sui"">secum</w>
                <w rend=""unknown"" n=""1"" pos=""CON"" msd=""MORPH=empty"" lemma=""cum3"">{secum}</w>
                <w rend=""unknown"" n=""1"" pos=""ADV"" msd=""Deg=Pos"" lemma=""simul1"">simul</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Numb=Plur|Mood=Sub|Tense=Pqp|Voice=Act|Person=3"" lemma=""defendo"">defendissent</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""1"" pos=""NOMcom"" msd=""Case=Dat|Numb=Sing"" lemma=""exitium"">exitio</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Case=Acc|Numb=Sing|Gend=MascNeut|Mood=Par|Tense=Fut|Voice=Act"" lemma=""sum1"">futurum</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma="";"">;</w>
                <w rend=""unknown"" n=""1"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""nam"">nam</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Dat|Numb=Sing"" lemma=""Caesar"">Caesari</w>
                <w rend=""unknown"" n=""1"" pos=""ADJqua"" msd=""Case=Acc|Numb=Plur|Gend=Masc|Deg=Pos"" lemma=""multus"">multos</w>
                <w rend=""unknown"" n=""1"" pos=""NOMpro"" msd=""Case=Acc|Numb=Plur"" lemma=""Marius"">Marios</w>
                <w rend=""unknown"" n=""1"" pos=""VER"" msd=""Mood=Inf|Tense=Pres|Voice=Act"" lemma=""insum1"">inesse</w>
                <w rend=""unknown"" n=""1"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""."">.</w>
                </ab>
            <ab type=""unknown"" n=""urn:cts:latinLit:phi1348.abo011.perseus-lat2:2"">
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Acc|Numb=Plur"" lemma=""stipendium"">Stipendia</w>
                <w rend=""unknown"" n=""2"" pos=""ADJord"" msd=""Case=Acc|Numb=Plur|Gend=Neut|Deg=Sup"" lemma=""primus"">prima</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""in"">in</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Abl|Numb=Sing"" lemma=""Asia"">Asia</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""facio"">fecit</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Gen|Numb=Sing"" lemma=""Marcus"">Marci</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Gen|Numb=Sing"" lemma=""Thermus"">Thermi</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Gen|Numb=Sing"" lemma=""praetor"">praetoris</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""contubernium"">contubernio</w>
                <w rend=""unknown"" n=""2"" pos=""PUNC"" msd=""MORPH=empty"" lemma="";"">;</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""ab"">a</w>
                <w rend=""unknown"" n=""2"" pos=""PROrel"" msd=""Case=Abl|Numb=Sing|Gend=MascNeut"" lemma=""qui1"">quo</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""ad2"">ad</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Case=Acc|Numb=Sing|Gend=Fem|Mood=Adj|Voice=Pass"" lemma=""arcesso"">accersendam</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""classis"">classem</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""in"">in</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Bithynia"">Bithyniam</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Gend=Masc|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""mitto"">missus</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""desido"">desedit</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""apud"">apud</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Acc|Numb=Sing"" lemma=""Nicomedes"">Nicomeden</w>
                <w rend=""unknown"" n=""2"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""2"" pos=""ADVneg"" msd=""MORPH=empty"" lemma=""non"">non</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""sine"">sine</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""rumor"">rumore</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Case=Gen|Numb=Sing|Gend=Fem|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""prosterno"">prostratae</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Dat|Numb=Sing"" lemma=""rex"">regi</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Dat|Numb=Sing"" lemma=""pudicitia"">pudicitiae</w>
                <w rend=""unknown"" n=""2"" pos=""PUNC"" msd=""MORPH=empty"" lemma="";"">;</w>
                <w rend=""unknown"" n=""2"" pos=""PROrel"" msd=""Case=Acc|Numb=Sing|Gend=Masc"" lemma=""qui1"">quem</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""rumor"">rumorem</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""augeo"">auxit</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""intra2"">intra</w>
                <w rend=""unknown"" n=""2"" pos=""ADJqua"" msd=""Case=Acc|Numb=Plur|Gend=Masc|Deg=Pos"" lemma=""paucus"">paucos</w>
                <w rend=""unknown"" n=""2"" pos=""ADV"" msd=""Deg=Pos"" lemma=""rursus"">rursus</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Acc|Numb=Plur"" lemma=""dies"">dies</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Gend=Fem|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""repeto"">repetita</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Nom|Numb=Sing"" lemma=""Bithynia"">Bithynia</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""per"">per</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Acc|Numb=Sing"" lemma=""causa"">causam</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Case=Gen|Numb=Sing|Gend=Fem|Mood=Adj|Voice=Pass"" lemma=""exigo"">exigendae</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Gen|Numb=Sing"" lemma=""pecunia"">pecuniae</w>
                <w rend=""unknown"" n=""2"" pos=""PUNC"" msd=""MORPH=empty"" lemma="","">,</w>
                <w rend=""unknown"" n=""2"" pos=""PROrel"" msd=""Case=Nom|Numb=Sing|Gend=Fem"" lemma=""qui1"">quae</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Mood=Sub|Tense=Impa|Voice=Pass|Person=3"" lemma=""debeo"">deberetur</w>
                <w rend=""unknown"" n=""2"" pos=""PROind"" msd=""Case=Dat|Numb=Sing|Gend=Com"" lemma=""quidam"">cuidam</w>
                <w rend=""unknown"" n=""2"" pos=""ADJqua"" msd=""Case=Dat|Numb=Sing|Gend=MascNeut|Deg=Pos"" lemma=""libertinus2"">libertino</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Dat|Numb=Sing"" lemma=""cliens"">clienti</w>
                <w rend=""unknown"" n=""2"" pos=""PROpos.ref"" msd=""Case=Dat|Numb=Sing|Gend=MascNeut"" lemma=""suus"">suo</w>
                <w rend=""unknown"" n=""2"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""."">.</w>
                <w rend=""unknown"" n=""2"" pos=""ADJqua"" msd=""Case=Nom|Numb=Sing|Gend=Fem|Deg=Pos"" lemma=""reliquus"">reliqua</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""militia"">militia</w>
                <w rend=""unknown"" n=""2"" pos=""ADJqua"" msd=""Case=Abl|Numb=Sing|Gend=Com|Deg=Comp"" lemma=""secundus1"">secundiore</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""fama"">fama</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Perf|Voice=Act|Person=3"" lemma=""sum1"">fuit</w>
                <w rend=""unknown"" n=""2"" pos=""CONcoo"" msd=""MORPH=empty"" lemma=""et2"">et</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""ab"">a</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Abl|Numb=Sing"" lemma=""Thermus"">Thermo</w>
                <w rend=""unknown"" n=""2"" pos=""PRE"" msd=""MORPH=empty"" lemma=""in"">in</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""expugnatio"">expugnatione</w>
                <w rend=""unknown"" n=""2"" pos=""NOMpro"" msd=""Case=Gen|Numb=Plur"" lemma=""Mytilenae"">Mytilenarum</w>
                <w rend=""unknown"" n=""2"" pos=""NOMcom"" msd=""Case=Abl|Numb=Sing"" lemma=""corona"">corona</w>
                <w rend=""unknown"" n=""2"" pos=""ADJqua"" msd=""Case=Abl|Numb=Sing|Gend=Fem|Deg=Pos"" lemma=""ciuicus"">ciuica</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Case=Nom|Numb=Sing|Gend=Masc|Mood=Par|Tense=Perf|Voice=Pass"" lemma=""dono"">donatus</w>
                <w rend=""unknown"" n=""2"" pos=""VER"" msd=""Numb=Sing|Mood=Ind|Tense=Pres|Voice=Act|Person=3"" lemma=""sum1"">est</w>
                <w rend=""unknown"" n=""2"" pos=""PUNC"" msd=""MORPH=empty"" lemma=""."">.</w>
                </ab>
            </body>
    </text>
</TEI>
");

    [Fact]
    public void TestMetaFields()
    {
        var file = new MockCanonFile("/tmp/example.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlData);
        var xmlParserFactory = new XmlParserFactory();
        using var lemmaLogging = new Logging(Path.Join(Directory.GetCurrentDirectory(), "logs"), "lemma");
        var doc = new LemmatizedDoc(file, xmlParserFactory, lemmaLogging);
        doc.Process();

        Assert.True(doc.GetDocumentID() == "phi1348.abo011", "No valid document ID found.");
        Assert.True(doc.GetTitle() == "Divus Julius", "No valid title found in English document.");
        Assert.True(doc.GetAuthor() == "Suetonius ca. 69-ca. 122", "No valid author found in Latin document.");
    }

    [Fact]
    public void TestSections1()
    {
        var file = new MockCanonFile("/tmp/example.xml", "phi1348.abo011",
            ICanonFile.Language.Latin, 2, xmlData);
        var xmlParserFactory = new XmlParserFactory();
        using var lemmaLogging = new Logging(Path.Join(Directory.GetCurrentDirectory(), "logs"), "lemma");
        var doc = new LemmatizedDoc(file, xmlParserFactory, lemmaLogging);
        doc.Process();

        var section = doc.GetSection("2");
        Assert.True(section != null, "Cannot find section '2'.");
        var tokens = section.GetTokens();
        Assert.True(tokens.Count == 60, $"Invalid token count. Expected 60, received {tokens.Count}.");
        var token = tokens[1];
        Assert.True(token.GetTokenType() == "ADJord", "Section 2, Token 2: Expected token "
            + $"type 'ADJord', received '{token.GetTokenType()}'.");
        var props = token.GetMsd();
        Assert.True(props.Count == 4, "Section 2, Token 2: Expected number of 'msd' properties is "
            + $"4, received '{props.Count}'.");
        Assert.True(props["Case"] == "Acc", "Section 2, Token 2: Expected 'Case' is 'Acc', "
            + $"received '{props["Case"]}'.");
    }
}
