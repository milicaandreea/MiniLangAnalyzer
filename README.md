# 🧠 MiniLang Analyzer (C# + ANTLR4)

A small C# application that uses ANTLR4 to parse and analyze a custom programming language defined in `MiniLang.g4`. The app reads source code from a text file and extracts information about global variables and functions (including main, parameters, return type, and control structures).

## 📁 Project Structure

- `MiniLang.g4` – Grammar definition for the custom language  
- `Program.cs` – Main C# program that loads the input code, builds the parse tree, and writes analysis results to text files

## 🧠 Features

- Reads source code from `input.txt`
- Builds a parse tree using ANTLR4-generated lexer and parser
- Extracts:
  - Global variable declarations (type, name, initial value)
  - Function declarations:
    - Name and return type
    - If it's recursive or iterative
    - Parameters
    - Local variables
    - Control structures (`if`, `for`, `while`)
- Results are saved in:
  - `functions.txt`
  - `global_variables.txt`

## ▶️ How to Use

1. Ensure you have the ANTLR runtime for C# installed
2. Generate the parser and lexer from `MiniLang.g4` using ANTLR
3. Place your MiniLang source code in `input.txt`
4. Run the project in Visual Studio or with `dotnet run`
5. Check `functions.txt` and `global_variables.txt` for output

## 📦 Dependencies

- [.NET 6.0+](https://dotnet.microsoft.com/)  
- [ANTLR4 Runtime for C#](https://www.nuget.org/packages/Antlr4.Runtime.Standard)
