using System;
using System.IO;

namespace project2
{

    class Lox {
        
        static Boolean hadError = false;

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

        private static void Run(string source) {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens) {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message) {
            Report(line, "", message);
        }

        private static void Report(int line, String where, String message) {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

    }


}