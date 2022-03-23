using System.Collections.Generic;

namespace project2
{

    class Scanner {
        private readonly string source;
        List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>{
            {"and", TokenType.AND},
            {"class", TokenType.CLA}
        };
        

        // might need column variable to be one ahead of current and pass this to other funtions

        public Scanner(string source) {
            this.source = source;
        }

        public List<Token> ScanTokens() {
            while (!IsAtEnd()) {

                start = current;
                ScanToken();   
            }
            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private void ScanToken() {
            char c = Advance();

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
                case ' ':
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

        private void Identifier() {
            while (IsAlphaNumeric(Peek())) {
                Advance();
            }

            AddToken(TokenType.IDENTIFIER);
        }

        private bool IsAlpha(char c) {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'z') || (c == '_')) {
                return true;
            }
            return false;
        }

        private bool IsAlphaNumeric(char c) {
            if ((char.IsDigit(c)) || (IsAlpha(c))) {
                return true;
            }
            return false;
        }

        private void Number() {
            while (IsDigit(Peek())) {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext())) {
                // Consume the '.'
                Advance();
                while (IsDigit(Peek())) {
                    Advance();
                }
            }

            AddToken(TokenType.NUMBER, source.Substring(start, current - start)); // may need to be a different length
        }

        private void String() {
            while (Peek() != '"' && !IsAtEnd()) {
                if (Peek() == '\n') {
                    line++;
                    Advance();
                } 
                if (IsAtEnd()) {
                    Lox.Error(line, "Unterminated String");
                    return;
                }

                Advance();

                string value = source.Substring(start + 1, current - start - 2); // Subtract 2 to remove quotes
                AddToken(TokenType.STRING, value);
            }
        }

        private bool Match(char expected) {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current ++;
            return true;
        }

        private char Peek() {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        private char PeekNext() {
            if (IsAtEnd()) {
                return '\0';
            }
            return source[current++];
        }

        private bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }

        private bool IsAtEnd() {
            return current >= source.Length;
        }

        private char Advance() {
            var prev = current;
            current++;
            return source[prev];
        }

        private void AddToken(TokenType type) {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal) {
            string text = source.Substring(start, current);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}