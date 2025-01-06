# Simple Compiler for Tiny Programming Language

## Overview

This project implements a **simple compiler** for a **tiny programming language** using **C#** and **WinForms**. The compiler consists of two main phases:  
1. **Lexeme Phase**: In this phase, the source code is tokenized into lexemes (the smallest unit of meaningful data such as keywords, operators, identifiers, etc.).
2. **Parser Phase**: The parser takes the lexemes produced by the lexer and constructs a **syntax tree** based on the grammar rules of the language.

The project focuses on providing a basic framework for understanding compiler design, including lexical analysis, syntax analysis, and building a simple interface using **WinForms** for user interaction.

## Features

- **Lexeme Phase (Lexer)**:  
  - Tokenizes the source code into lexemes using regular expressions.
  - Identifies keywords, operators, identifiers, and literals.
  
- **Parser Phase**:  
  - Parses the tokenized lexemes into a syntax tree according to the language's grammar.
  - Implements a simple recursive descent parser to process the syntax and handle errors.

- **WinForms Interface**:  
  - Provides a user-friendly interface to input source code, view tokenized lexemes, and display parsing results.
  - Includes error messages and feedback for users to understand syntax errors.

## Requirements

- **Software**:  
  - **C#** for compiler implementation.
  - **WinForms** for the graphical user interface.
  - **.NET Framework** (or .NET Core) for the development environment.

- **Libraries/Tools**:  
  - **Visual Studio** (or another C# IDE) for compiling and running the project.
  - **Regular Expressions (Regex)** for lexical analysis.

## Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/simple-compiler.git
    ```

2. Navigate into the project directory:
    ```bash
    cd simple-compiler
    ```

3. Open the project in **Visual Studio** or your preferred **C# IDE**.

4. Build and run the project:
    - Ensure that all required dependencies are installed (if any).
    - Compile the project using the IDE.

## Usage

1. **Input Source Code**:  
   - Open the **WinForms application**, and in the input field, type or paste the source code in the tiny programming language.

2. **Lexeme Phase**:  
   - After entering the code, click on the "Tokenize" button to initiate the **lexical analysis**. The application will display a list of lexemes (tokens) such as keywords, operators, and identifiers.

3. **Parser Phase**:  
   - Click the "Parse" button to run the **parser**. The syntax tree and parsing results will be displayed. Any syntax errors will be highlighted with an error message indicating the line and column where the issue occurred.

4. **Error Handling**:  
   - Syntax errors encountered during parsing are shown with the exact location of the error in the code.

## Components

- **Lexer (Lexical Analysis)**:  
  - This phase converts the raw source code into a series of lexemes using regular expressions for matching patterns like keywords, numbers, identifiers, and operators.

- **Parser (Syntax Analysis)**:  
  - The parser reads the lexemes produced by the lexer and checks them against the syntax rules of the language. The parser builds a **syntax tree** to represent the structure of the source code.

