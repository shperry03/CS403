using System;
using System.IO;
using System.Collections.Generic;

namespace project2
{

    class Lox {
        
        // Interpreter to use to call on Lox statements
        private static readonly Interpreter interpreter = new Interpreter();

        // Error flag to set and check
        static Boolean hadError = false;

        // Runtime Error flag to set and check
        static Boolean hadRuntimeError = false;

        
        // Read terminal input and run if correct
        public static void Main(string[] args) {

            if (args.Length > 1) {
                Console.WriteLine("Usage: jlox [script]");
                System.Environment.Exit(64);

            } else if (args.Length == 1) { // Run on a Lox file
                RunFile(args[0]);
            } else { // Run on one line of Lox input at a time
                RunPrompt();
            }
        }

        /*
        Execute code from filepath
        */
        private static void RunFile(string path) {
            // Read in the text from the filepath
            var sourceFile = File.ReadAllText(path);

            // Call helper method to interpret Lox
            Run(sourceFile);
            
            // System exit codes based on error type
            if (hadError) {
                System.Environment.Exit(65);
            }
            if (hadRuntimeError) {
                System.Environment.Exit(70);
            }
        }

        /*
        Execute code one line at a time
        */
        private static void RunPrompt() {
            
            // Infinite console loop
            while(true) {
                // Read in a line of lox code from the user and call the helper Run() method
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) break;
                Run(line);
                hadError = false; // reset error flag 
            }
        }
        
        /*
        Scans tokens, then creates a list of Stmt and calls the interpret method on statement
        */
        private static void Run(string source) {

            // Create scanner and call ScanTokens()
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            // Create and call a parser on the list of Tokens we found    
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            // Stop if there was a syntax error.
            if (hadError) {
                return;
            }

            // Interpret the Lox code of each Stmt
            interpreter.interpret(statements);
        }

        // Call Report() to provide error information to user
        public static void Error(int line, string message) {
            Report(line, "", message);
        }

        /*
        Report runtime error and tell the user the line
        */
        public static void RuntimeError(RuntimeError error) {
            Console.WriteLine(error.Message + "\n[line " + error.token.line + "]");
        }

        /*
        Directly write error message to console and set error flag
        */
        private static void Report(int line, String where, String message) {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

        public static void Error(Token token, string message) {
            if (token.type == TokenType.EOF) {
                Report(token.line, " at end", message);
            } else {
                Report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

    }


}