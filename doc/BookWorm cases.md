The Canonical XML documents are mostly handwritten, and in many of them the
section markers and the text blocks appear in a weird order.
There are also differences between the English and Latin versions of the documents.
This doc shows some of the cases and defines an algorithm for the parsing.

- [Normal case](#normal-case)
- [Reverse case](#reverse-case)
- [Floating case](#floating-case)
- [Reverse-floating case](#reverse-floating-case)
- [Incomplete case](#incomplete-case)
- [Chosen parsing algorithm](#chosen-parsing-algorithm)
  - [Main containers](#main-containers)
  - [Algorithm](#algorithm)

## Normal case

- [book_1, chapter_1, text_1], [chapter_2, text_2]
- Repeated section markers can be ignored:
  - [book_1, chapter_1, text_1, chapter_1, text_2]

## Reverse case

- [chapter_1, book_1, text_1], [chapter_2, text_2], [chapter_3, book_2, text_3]
- Possible misinterpretation: Empty chapter_3:
  - [chapter_1, book_1, text_1], [chapter_2, text_2], [chapter_3], [book_2, text_3]
- The proper grouping can be inferred if the parsing ignores the hierarchy of the sections,
  and instead: It starts a new section only if an already set level was changed to
  a new value.

## Floating case

- The floating text is usually a title.
- [book_1, text_1, chapter_1, text_2], [chapter_2, text_3,
    book_2, text_4], [chapter_1, text_5]
- Possible misinterpretation:
  - [book_1, text_1, chapter_1, text_2], [chapter_2, text_3],
    [book_2, text_4, chapter_1], [text_5]
- The solution is the same as for the "reverse case".

## Reverse-floating case

- This can only happen at the beginning of a document, because the "book" was
  still unknown, which keeps the elements floating: (There was no "pivot section".)
  - [chapter_1, text_1, book_1], [chapter_2, text_2], [book_2, ...]
- Possible misinterpretations:
  - [chapter_1, text_1, book_1], [chapter_2, text_2, book_2, ...]
  - [chapter_1, text_1], [book_1, chapter_2, text_2], [book_2, ...]
- If the "text_2" was missing, then the second section would become a simple "reverse case":
  - [chapter_1, text_1, book_1], [chapter_2, book_2, ...]

## Incomplete case

- [book_1, chapter_1, text_1], [chapter_2, section_1, text_2], [chapter_3, text_3]
- This is very common as section markers for higher levels (like "book")
  are usually not repeated.
- The missing parts can be taken from the pivot. (see below)

## Chosen parsing algorithm

### Main containers

- Pivot section:
  - Example items: "book=1|chapter=1|section=12", "book=2|chapter=4|section=1".
  - The last complete section. The floating section gets missing parts (like "book") from this.
  Empty until the first completion. Gets a new value after each completion.
- Floating section:
  - Example items: "chapter=2|section=1", "section=23", "book=4|chapter=1"
  - Becomes empty after each completion and accumulates markers in between.
- Floating elements:
  - The elemets (texts) that appeared between completions.

### Algorithm

- Collect section markers in the "floating section" until a completion.
- Collect elements in "floating elements" until completion.
- A completion happens if any of the following is true: (In these cases the new marker is part of the next section.)
  - We encounter a section marker which is already present in "floating section"
    and would set the level to a new value.
  - The end of the document is reached.
  - A newly encountered marker has higher level than any previous markers in "floating section",
    AND that marker is persent in the pivot (meaning: not reverse-floating.)
    AND at least one element (text) appeared in the current section.
- After completion, the levels in the "floating section" are put into the pivot.
  The missing parts are left unchanged. The "floating elements" are registered under this new pivot.
- The "floating elements" container is cleared.
- The "floating section" container is cleared.
- Continue from the beginning unless the document has ended.
