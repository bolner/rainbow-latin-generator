Rainbow Latin Reader
====================

- [Rainbow Latin Reader](#rainbow-latin-reader)
- [Sources](#sources)
- [Usage](#usage)
- [Unit tests](#unit-tests)
- [Development setup](#development-setup)

# Sources

- Lemmatized Latin texts:
  - https://github.com/lascivaroma/latin-lemmatized-texts/
- Collection of dual-language Latin texts from the Perseus Digital Library:
  - https://github.com/PerseusDL/canonical-latinLit
- Whitaker's WORDS
  - https://github.com/mk270/whitakers-words

# Usage

- Inside the `RainbowLatinReader` directory: Copy the `config.ini.template` as `config.ini` and fill in the missing paths.
- Create the output directory that is specified as `output.dir` in the config file. (You can create an `output` folder inside the project directory, as that is already added to the `.gitignore` file.)
- Before you compile Whitaker's Words, change this setting:
  - File: `src/support_utils/support_utils-developer_parameters.adb`
  - Required settings:
    - `Pause_In_Screen_Output => False,`
    - `Omit_Archaic => False`
    - `Omit_Uncommon => False`
    - `Minimize_Output => False`
- Then execute:
    ```bash
      cd RainbowLatinReader
      dotnet run
    ```
- The results will be generated in the output folder.

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
