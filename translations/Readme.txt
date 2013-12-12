This folder contains the master file for all the translations (translations.ods)
as well as any loadable tranlations created from it.

Translations.ods is formatted with the language name in the first row, and all
translated strings for that language in the rows below. The first column
contains the ID for the strings in that row. Language ini files are generated
by concatenating the first column, an equals sign and the column under the
language name. (see "English (EN-US).ini" for an example) The string replacements
column gives some context for what the replacement will contain.

The TranslationBuilder project included with this source code can build all the
translations from the spreadsheet automatically.