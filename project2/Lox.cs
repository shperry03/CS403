using System;
using System.IO;
using System.Collections.Generic;

namespace project2
{

    class Lox {
        
        private static readonly Interpreter interpreter = new Interpreter();
        static Boolean hadError = false;
        static Boolean hadRuntimeError = false;

        
        // Read terminal input and run if correct
        public static void Main(string[] args) {

            if (args.Length > 1) {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);

            } else if (args.Length == 1) {
                RunFile(args[0]);
            } else {
                RunPrompt();
            }
        }

        /*
        Execute code from filepath
        */
        private static void RunFile(string path) {
            var sourceFile = File.ReadAllText(path);
            Run(sourceFile);
            // Add error handling
            if (hadError) {
                Environment.Exit(65);
            }
            if (hadRuntimeError) {
                Environment.Exit(70);
            }
        }

        /*
        Execute code one line at a time
        */
        private static void RunPrompt() {

            while(true) {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) break;
                Run(line);
                hadError = false; // reset error flag 
            }
        }
        
        // Main execution command
        private static void Run(string source) {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            // Stop if there was a syntax error.
            if (hadError) {
                return;
            }

            interpreter.interpret(statements);
        }

        // Error method, just calls report for now
        public static void Error(int line, string message) {
            Report(line, "", message);
        }

        public static void RuntimeError(RuntimeError error) {
            Console.WriteLine(error.Message + "\n[line " + error.token.line + "]");
        }

        // Report an error message to console
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