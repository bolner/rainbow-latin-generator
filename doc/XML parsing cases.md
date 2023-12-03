# Canonical Literature: Special cases

- Don't auto-fetch text at destinations. (Can't capture attributes if we re-position.)
- Capture text since the last position.
- Rendering markup:
  - `<quote>new man</quote>`
  - `<hi rend="sup">2</hi>`
  - `<hi rend="italics">T</hi>`
  - `<hi rend="sup">2</hi>`
  - `<reg>consulatum</reg>` ???
- Typos: `<p>>LIVIA having married Augustus when she`
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
- This is part of the text: `<label type="opener">Item eiusdem ad eundem quomodo substantiae in eo quod sint bonae sint cum non sint substantialia bona</label>` (Would be captured, so no problem.)
- References to source page? `<pb id="p.41"/>`
- Not part of the text: `<div type="commentary" resp="ed">`, `<div type="commentary">`
- DIV types:
  - `<div type="textpart" subtype="section" n="1" resp="perseus">`
  - `<div type="textpart" n="3" subtype="section">`
  - `<div type="textpart" subtype="book" n="2">`
  - `<div type="textpart" n="1" subtype="chapter">`
  - `<div type="textpart" n="1" subtype="letter">`
- Other section markers:
  - Segment: `<seg type="section" n="12">...</seg>`