- update the subject-object diagram
- Use all 3 milestones and allow to group the text by any of them:
  - book, chapter, section
  - Then ability to get statistics:
    - Unique milestone IDs for a level.
    - Total milestone count for a level.
  - Use these statistics to connect Canonical with Lemmatized.
- Solve issue with undefined HTML entities.
- Extend the document version matching to all 3 types. ???
- Lookup for text parts by multiple locators (chapter, section): ???
  ```xml
    <div n="2" type="textpart" subtype="chapter"><div n="1" type="textpart" subtype="section">
    <p>Stipendia prima in Asia fecit Marci Thermi praetoris contubernio; a quo ad accersendam
        classem in Bithyniam missus desedit apud Nicomeden, non sine rumore prostratae regi
        pudicitiae; quem rumorem auxit intra paucos rursus dies repetita <placeName key="Bithynia">Bithynia</placeName> per causam exigendae pecuniae, quae deberetur cuidam libertino
        clienti suo. reliqua militia secundiore fama fuit et a Thermo in expugnatione Mytilenarum
        corona ciuica donatus est.</p>
    </div></div>
  ```
- Remove CanonLitSection and LemmatizedSection. Because the Librarian handles the grouping of elements.
- Handle nested sections (book/chapter, no "/>" at the end):
  ```
  <text><body>
  <pb id="p.129"/>
  <div1 type="book" n="1">
  <div2 type="chapter" n="praef"><p>
  ```

Find all XML/HTML entities:

```bash
grep -RhoE --include "*.xml" "\&[a-zA-Z0-9]+\;" /mnt/nvme/projects/canonical-latinLit | sort | uniq
```

Replace all
```bash
dotnet run /mnt/nvme/projects/canonical-latinLit-mine
```


Issues: 

stoa0040/stoa0040.stoa011.perseus-eng1.xml
extra: </titleStmt>

stoa0089/stoa001/stoa0089.stoa001.perseus-eng1.xml
<pubPlace>London, England<</pubPlace>

Double closing for tags:
```bash
grep -RoE --include "*.xml" ".{0,10}>>.{0,10}" /mnt/nvme/projects/canonical-latinLit
```