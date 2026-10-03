# Book Cataloging System (C# and Ruby)

A console application that manages a catalog of books. The **same application is written twice**, once in **C#** and once in **Ruby**, so the two languages can be compared (group project: cross-language application development, Project Option 4).

| Language | Owner | Folder |
|----------|-------|--------|
| C# (.NET 8) | Anulekha | `csharp/` |
| Ruby 3 | Dushyanth | `ruby/` |

Both programs have the same menu, the same rules, the same messages, and read and write the same JSON file (`data/books.json`).

## Features

- **Store books** with an ID, title, author, genre, and publication year.
- **Add** a book (all fields validated) and **remove** a book by ID.
- **List** all books.
- **Search** by title, author, or genre (case-insensitive, partial matches, sorted by title).
- **Reports**: books grouped by genre or by author, with a count for each group.
- **Persistence**: the catalog loads from `data/books.json` when the program starts and is saved when you choose "Save and exit".
- **Error handling**: bad menu input, empty fields, invalid years, unknown IDs, and missing or corrupt data files are all handled without crashing.

## Repository structure

```
book-catalog-csharp-ruby/
├── README.md
├── data/
│   └── books.json              Sample catalog (10 books) used by both programs
├── csharp/
│   ├── BookCatalog.sln
│   ├── BookCatalog/            C# application
│   │   ├── Book.cs             Book class and BookGroup record
│   │   ├── Validation.cs       Input rules
│   │   ├── Catalog.cs          Collection, add/remove, LINQ search and reports
│   │   ├── Storage.cs          JSON load/save (System.Text.Json)
│   │   └── Program.cs          Console menu
│   └── BookCatalog.Tests/      xUnit unit tests
├── ruby/
│   ├── main.rb                 Console menu (start here)
│   ├── book.rb                 Book class and BookGroup struct
│   ├── validation.rb           Input rules
│   ├── catalog.rb              Collection, add/remove, block-based search and reports
│   ├── storage.rb              JSON load/save (json library)
│   └── test/                   Minitest unit tests
├── tests/
│   ├── compare.sh              Runs both programs on the same input and compares results
│   ├── scenarios.txt           List of test scenarios
│   ├── inputs/                 Scripted keyboard input for each scenario
│   └── fixtures/               Sample and deliberately broken data files
└── docs/                       Reports and screenshots
```

## Requirements

| Tool | Version | Check with |
|------|---------|------------|
| .NET SDK | 8.0 or later | `dotnet --version` |
| Ruby | 3.0 or later | `ruby -v` |
| Bash (only for `tests/compare.sh`) | any | Included with Mac/Linux; use **Git Bash** on Windows |

No extra libraries are needed to run either program. The C# unit tests download xUnit automatically the first time you run them (internet connection required).

## How to run

Start from the repository root (the folder that contains this README).

### C#

```
cd csharp/BookCatalog
dotnet run
```

`dotnet run` compiles the program and starts it. To only compile, use `dotnet build`.

### Ruby

```
cd ruby
ruby main.rb
```

### Using a different data file

Both programs accept an optional path to a JSON file:

```
dotnet run -- path/to/other.json      (C#, run from csharp/BookCatalog)
ruby main.rb path/to/other.json       (Ruby, run from ruby)
```

If the file does not exist, the program starts with an empty catalog and creates the file when you save.

## Using the program

```
=== Book Catalog ===
1. Add a book
2. Remove a book
3. List all books
4. Search by title
5. Search by author
6. Search by genre
7. Report: books by genre
8. Report: books by author
9. Save and exit
Choose an option:
```

Type a number and press Enter. **Changes are only saved when you choose option 9.**

Example (search by author):

```
Choose an option: 5
Search author: orwell
=== Search Results ===
[2] 1984 | George Orwell | Dystopian | 1949
[3] Animal Farm | George Orwell | Political Satire | 1945
```

Example (report by genre, shortened):

```
=== Books by Genre ===
Dystopian (2)
  [10] Brave New World | Aldous Huxley | Dystopian | 1932
  [2] 1984 | George Orwell | Dystopian | 1949
Fantasy (2)
  [1] The Hobbit | J.R.R. Tolkien | Fantasy | 1937
  [8] The Fellowship of the Ring | J.R.R. Tolkien | Fantasy | 1954
```

## Rules shared by both programs

| Item | Rule |
|------|------|
| Book display | `[id] Title \| Author \| Genre \| Year` |
| ID | First book is 1; a new book gets the highest existing ID plus one |
| Validation | Title, author, and genre cannot be empty or only spaces. Year must be a whole number (digits only) from 1000 through the current year |
| Search | Query is trimmed and cannot be empty; matches anywhere in the field, ignoring case; results sorted by title |
| Reports | Groups sorted alphabetically (ignoring case, so "Sci-Fi" and "sci-fi" are one group); books in a group sorted by year, then title; each heading shows the count |
| List all | Sorted by ID |
| Messages | `Book added.` `Book removed.` `No book found with ID n.` `No books found.` `Invalid input.` `Catalog saved. Goodbye.` |
| Data file problems | Missing: `Data file not found. Starting with an empty catalog.` Corrupt: `Warning: could not read data file. Starting with an empty catalog.` |
| End of input | If input ends (for example, in a script), the program saves and exits as if you chose option 9 |

A data file counts as **corrupt** if it is not valid JSON, is not an array, or any book has a missing or wrong-type field, an invalid year, an ID below 1, or a duplicate ID.

JSON format (`data/books.json`):

```json
[
  {
    "id": 1,
    "title": "The Hobbit",
    "author": "J.R.R. Tolkien",
    "genre": "Fantasy",
    "year": 1937
  }
]
```

## Language-specific features

| Feature | C# (`csharp/BookCatalog`) | Ruby (`ruby/`) |
|---------|--------------------------|----------------|
| Book type | `class Book` with typed `init` properties | `class Book` with `attr_reader`, no types |
| Collection | `List<Book>`; `Catalog` implements `IEnumerable<Book>` | `Array`; `Catalog` includes `Enumerable` and defines `each` |
| Search | LINQ: `Where`, `OrderBy`, `ThenBy` | Blocks: `select`, `sort_by` |
| Reports | LINQ: `GroupBy`, `OrderBy`, `Select` | Blocks: `group_by`, `sort_by`, `map` |
| Passing behavior | `Func<Book, string>` lambdas and delegates | Blocks and `&:symbol` shorthand |
| Group data | `record BookGroup` | `Struct.new(:name, :books)` |
| Typing | Static; checked at compile time; nullable reference types | Dynamic; types checked when running, so validation checks types by hand |
| JSON | `System.Text.Json` maps JSON to typed objects | `json` library gives Hashes; converted to `Book` by hand |
| Errors | `try` / `catch` with exception filters | `begin` / `rescue` (method-level `rescue`) |
| Unit tests | xUnit | Minitest |

## Testing

### 1. Compare both programs (recommended)

```
bash tests/compare.sh
```

This builds the C# program, then runs **19 scenarios** through both programs using the same scripted input (listing, searching, adding, removing, invalid input, empty catalog, mixed capitalization, accented characters, missing and corrupt data files). For each scenario it checks that:

1. the text shown on screen is identical, and
2. the JSON file each program saves is identical.

Expected last lines:

```
Passed: 19   Failed: 0
```

The scenarios use copies of the files in `tests/fixtures/`, so your real `data/books.json` is never changed.

### 2. Unit tests

```
cd csharp
dotnet test
```

```
cd ruby
ruby test/all_tests.rb
```

Each language has 17 matching unit tests covering validation, adding, removing, searching, reports, and loading/saving files.

### Bug found and fixed during testing

The comparison test showed that Ruby saved an empty catalog as `[` + blank line + `]` while C# saved `[]`. Cause: the `JSON.pretty_generate` method in the Ruby `json` library version we used formats empty arrays differently. Fix: `Storage.to_json_text` in `ruby/storage.rb` writes `[]` directly when the catalog is empty. Both programs now produce identical files.

## Team

- **Anulekha**: C# implementation (`csharp/`), C# tests
- **Dushyanth**: Ruby implementation (`ruby/`), Ruby tests, repository setup
