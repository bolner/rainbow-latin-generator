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
using RainbowLatinReader;

namespace unit_tests;


sealed class MockSystemProcess : ISystemProcess {
    public void Start(string arguments) {

    }

    public string Read() {
        return @"exigu.ae             ADJ    1 1 GEN S F POS             
exigu.ae             ADJ    1 1 DAT S F POS             
exigu.ae             ADJ    1 1 NOM P F POS             
exigu.ae             ADJ    1 1 VOC P F POS             
exiguus, exigua, exiguum  ADJ   [XXXBX]  
small; meager; dreary; a little, a bit of; scanty, petty, short, poor;

amic.orum            N      2 1 GEN P M                 
amicus, amici  N (2nd) M   [XXXBO]  
friend, ally, disciple; loved one; patron; counselor/courtier (to a prince);
amic.orum            ADJ    1 1 GEN P M POS             
amic.orum            ADJ    1 1 GEN P N POS             
amicus, amica -um, amicior -or -us, amicissimus -a -um  ADJ   [XXXAO]  
friendly, dear, fond of; supporting (political), loyal, devoted; loving;

qqqqqqqq                         ========   UNKNOWN    

copi.ae              N      1 1 GEN S F                 
copi.ae              N      1 1 LOC S F                 
copi.ae              N      1 1 DAT S F                 
copi.ae              N      1 1 NOM P F                 
copi.ae              N      1 1 VOC P F                 
copia, copiae  N (1st) F   [XXXAO]  
plenty, abundance, supply; troops (pl.), supplies; forces; resources; wealth;
number/amount/quantity; sum/whole amount; means, opportunity; access/admission;
copy;

s.unt                V      5 1 PRES ACTIVE  IND 3 P    
sum, esse, fui, futurus  V   [XXXAX]  
be; exist; (also used to form verb perfect passive tenses) with NOM PERF PPL

cum                  ADV    POS                         
cum  ADV   [XXXAO]  
when, at the time/on each occasion/in the situation that; after; since/although
as soon; while, as (well as); whereas, in that, seeing that; on/during which;
cum                  PREP   ABL                         
cum  PREP  ABL   [XXXAO]  
with, together/jointly/along/simultaneous with, amid; supporting; attached;
under command/at the head of; having/containing/including; using/by means of;

adversari.o          N      2 4 DAT S N                 
adversari.o          N      2 4 ABL S N                 
adversarium, adversari(i)  N (2nd) N   [XXXEO]    uncommon
temporary memorandum/account/day book (pl.); opponent's arguments/assertions;
adversari.o          N      2 4 DAT S C                 
adversari.o          N      2 4 ABL S C                 
adversarius, adversari(i)  N (2nd) C   [XXXBO]  
enemy, adversary, antagonist, opponent, rival, foe; of an opposing party;
adversari.o          ADJ    1 1 DAT S M POS             
adversari.o          ADJ    1 1 DAT S N POS             
adversari.o          ADJ    1 1 ABL S M POS             
adversari.o          ADJ    1 1 ABL S N POS             
adversarius, adversaria, adversarium  ADJ   [XXXCO]  
opposed (to), hostile, inimical, adverse; harmful, injurious, prejudicial;

issi                 SUFFIX                             
-est, most ~, much ~, makes SUPER;
gratiosissi.mo       ADJ    0 0 DAT S M SUPER           
gratiosissi.mo       ADJ    0 0 DAT S N SUPER           
gratiosissi.mo       ADJ    0 0 ABL S M SUPER           
gratiosissi.mo       ADJ    0 0 ABL S N SUPER           
gratiosus, gratiosa, gratiosum  ADJ   [XXXDX]    lesser
agreeable, enjoying favor; kind;

contend.at           V      3 1 PRES ACTIVE  SUB 3 S    
contendo, contendere, contendi, contentus  V (3rd)   [XXXAO]  
stretch, draw tight, make taut; draw/bend (bow/catapult); tune; stretch out;
compete/contend (fight/law), dispute; compare/match/contrast; demand/press for;
strain/tense; make effort, strive for; speak seriously/passionately; assert;
hurl, shoot; direct; travel; extend; rush to, be in a hurry, hasten;




";
    }

    public void Write(string data) {

    }

    public void WaitForExit() {

    }

    public void Dispose() {
        
    }
}
