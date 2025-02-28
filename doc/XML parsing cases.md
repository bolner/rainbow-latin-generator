# Canonical Literature: Special cases

- Rendering markup:
  - `<quote>new man</quote>`
  - `<hi rend="sup">2</hi>`
  - `<hi rend="italics">T</hi>`
  - `<hi rend="sup">2</hi>`
  - `<reg>consulatum</reg>` ???
- Typos:
  - `<p>>LIVIA having married Augustus when she`
- Milestones:
  - `<milestone unit="loebline" n="10"/>`
  - `<milestone n="7" unit="section"/>`
  - `<milestone n="1" unit="chapter"/>`
  - `<milestone unit="para"/>`
  - `<milestone unit="paragraph" n="62" />`
  - In same document: `<milestone unit="section" n="104"/>` `<milestone unit="paragraph" n="58" />`
  - `<milestone unit="line" ed="exclude" n="365"/>`
  - `<milestone unit="card" n="703"/>`.
- Line: `<l n="703">Nec vero ipse metus curasque resolvere ductor,</l>`
- This is part of the text: `<label type="opener">Item eiusdem ad eundem quomodo substantiae in eo quod sint bonae sint cum non sint substantialia bona</label>`
- Not part of the text: `<div type="commentary" resp="ed">`, `<div type="commentary">`
- DIV types:
  - `<div type="textpart" subtype="section" n="1" resp="perseus">`
  - `<div type="textpart" n="3" subtype="section">`
  - `<div type="textpart" subtype="book" n="2">`
  - `<div type="textpart" n="1" subtype="chapter">`
  - `<div type="textpart" n="1" subtype="letter">`
  - `<div1 type="book" n="1"><p>`
- Other section markers:
  - Segment: `<seg type="section" n="12">...</seg>`
  - `<seg type="section" n="12">`
- Page numbers (usually different in Latin and English):
  ```
    <pb id="p.3"/>
  ```
- In `phi0631.phi001`:
  - `<div2 type="poem" n="25">`
- Abbreviations with expansions:
  - `<abbr><expan>suppromus es</expan>suppromu's</abbr>`
- "del" tags to be removed:
  - `ex<del>eo</del>cogitauit`
  - `ciui<del>s</del> uacationem`
