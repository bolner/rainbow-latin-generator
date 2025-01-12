Rainbow Latin Generator
=======================

- [Rainbow Latin Generator](#rainbow-latin-generator)
- [Introduction](#introduction)
- [Credits](#credits)
- [Usage](#usage)
- [Architecture](#architecture)
- [Unit tests](#unit-tests)
- [Development setup](#development-setup)

# Introduction

This software generates Latin-English dual-language documents. See main webpage:
- https://bolner.github.io/rainbow-latin-reader/index.html

Watch the introduction video on YouTube:

[![Rainbow Latin Reader, Introduction](doc/youtube_intro.png)](https://www.youtube.com/watch?v=9ufyqLxwcEE)

# Credits

Rainbow Latin Reader was created by [Tamas Bolner](https://github.com/bolner)
and published under the [Apache License](http://www.apache.org/licenses/LICENSE-2.0)
(Copyright 2024). It is based on the following 3 projects:

- Perseus Canonical Latin literature:
  - https://github.com/PerseusDL/canonical-latinLit
- Whitaker's WORDS
  - https://github.com/mk270/whitakers-words
- Lemmatized Latin texts by Thibault Clérice et al.:
  - https://github.com/lascivaroma/latin-lemmatized-texts/

# Usage

- Inside the `RainbowLatinReader` directory: Copy the `config.ini.template` as `config.ini` and fill in the missing paths.
- Create the output directory that is specified as `output.dir` in the config file. (You can create an `output` folder inside the project directory, as that is already added to the `.gitignore` file.)
- Before you compile Whitaker's Words, change these settings:
  - File: `src/support_utils/support_utils-developer_parameters.adb`
  - Required settings:
    - `Pause_In_Screen_Output => False,`
    - `Omit_Archaic => False`
    - `Omit_Uncommon => False`
    - `Minimize_Output => False`
- Then after you compiled WORDS, execute Rainbow Latin Reader:
    ```bash
      cd RainbowLatinReader
      dotnet run
    ```
- The results will be generated in the output folder:

```
$ dotnet run
- 18:12:47 - Started. Max parallel threads: 44.
- 18:12:47 - Parsing canonical documents.
- 18:12:51 - Successfully parsed 56 Canonical documents. (Out of 336)
- 18:12:51 - Parsing lemmatized documents.
- 18:13:02 - Collecting Latin words for the dictionary lookups.
- 18:13:02 - Starting word lookups in Whitaker's Words.
- 18:13:06 - Done. Total unique Latin word count: 46884.
- 18:13:06 - There are 2 Canonical document pairs for which no lemmatized version exists.
- 18:13:06 - Generating pages.
- 18:13:13 - Generating index page.
- 18:13:13 - Completed. Total HTML pages generated: 160.
- 18:13:13 - Changed: 1, unchanged: 159.
$ 
```

# Architecture

Click on the image for higher resolution:

[<img src="doc/Subject-Object%20Diagram%20small.png">](doc/Subject-Object%20Diagram.png)

# Unit tests

Running all unit tests:
```bash
cd unit_tests
dotnet test
```

# Development setup

- [VS Code](https://code.visualstudio.com) (Microsoft)
- Plugins:
  - [C# - Base language support for C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) (Microsoft)
  - [Handlebars](https://marketplace.visualstudio.com/items?itemName=andrejunges.Handlebars) (André Junges)
