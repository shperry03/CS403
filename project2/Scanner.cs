using System.Collections.Generic;

namespace project2
{

    class Scanner {
        private readonly string source;
        List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        // Dicitonary to read from Lox language token input to Type
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>{
            {"and", TokenType.AND},
            {"class", TokenType.CLASS},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"for", TokenType.FOR},
            {"fun", TokenType.FUN},
            {"if", TokenType.IF},
            {"nil", TokenType.NIL},
            {"or", TokenType.OR},
            {"print", TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super", TokenType.SUPER},
            {"this", TokenType.THIS},
            {"true", TokenType.TRUE},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE}
        };
        



        // might need column variable to be one ahead of current and pass this to other funtions

        public Scanner(string source) {
            this.source = source;
        }

        // Read all tokens until end of file
        public List<Token> ScanTokens() {
            while (!IsAtEnd()) { // Checking for lexemes
                
                start = current;
                ScanToken();   
            }
            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        /*
            Scan a single token (character) and call the appropriate method if needed
        */

        private void ScanToken() {
            char c = Advance(); // Gets the current character then adds 1

            switch (c) {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/')) {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    } else {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ': // need to break here?
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '"':
                    String();
                    break;
                default:
                    if (IsDigit(c)) {
                        Number();
                    } else if (IsAlpha(c)) {
                        Identifier();
                    } else {
                        Lox.Error(line, "Unexpected character.");    
                    }
                    break;
            }
        }

        // If the item is not in our dictionary, then it must be an identifier
        private void Identifier() {
            while (IsAlphaNumeric(Peek())) {
                Advance();
            }

            // Check the substring and add it to our dictionary
            var text = source.Substring(start, current - start); 
            TokenType type;
            if (!keywords.ContainsKey(text)) { // may need to change this somehow to check for a null value?
                type = TokenType.IDENTIFIER;
            } else {
                type = keywords[text];
            }
            AddToken(type);
        }


        // Check if in the alphabet or an underscore        
        private bool IsAlpha(char c) {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'z') || (c == '_')) {
                return true;
            }
            return false;
        }


        // Check if in the alphabet, underscore, or is a number
        private bool IsAlphaNumeric(char c) {
            if ((char.IsDigit(c)) || (IsAlpha(c))) {
                return true;
            }
            return false;
        }

        // Get an entire number with decimal
        private void Number() {
            while (IsDigit(Peek())) {
                Advance();
            }

            // If we reach a decimal while scanning the number, 
            if (Peek() == '.' && IsDigit(PeekNext())) {
                // Consume the '.'
                Advance();
                while (IsDigit(Peek())) {
                    Advance();
                }
            }
            AddToken(TokenType.NUMBER, double.Parse(source.Substring(start, current - start))); // may need to be a different length and cast to number?
        }

        // Read until end of string 
        private void String() {

            // Check for end quote operator
            while (Peek() != '"' && !IsAtEnd()) {
                // Advance if at end of line
                if (Peek() == '\n') {
                    line++;
                }
                Advance();
            }
            // If at the end of the file
            if (IsAtEnd()) {
                Lox.Error(line, "Unterminated String");
                return;
            }

            // Advance to next character which is the end quote 
            Advance();

            string value = source.Substring(start + 1, current - start - 2); // Subtract 2 to remove quotes
            AddToken(TokenType.STRING, value);
        }
        

        // Check if current char matches the passed in char
        private bool Match(char expected) {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;
            // Return true and increment if the character matches
            current ++;
            return true;
        }

        // Return the next character
        private char Peek() {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        // Return 2 characters ahead if we can
        private char PeekNext() {
            if (current + 1 >= source.Length) {
                return '\0';
            }
            return source[current+1];
        }


        // Check if input is number
        private bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }


        // Check if at the end of file
        private bool IsAtEnd() {
            return current >= source.Length;
        }

        // Advance current and return the old value
        private char Advance() {
            var prev = current;
            current++;
            return source[prev];
        }

        // Add an empty token to the dictionary
        private void AddToken(TokenType type) {
            AddToken(type, null);
        }


        // Add a token to the dictionary with a value
        private void AddToken(TokenType type, object literal) {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}