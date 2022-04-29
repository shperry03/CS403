using System.Collections.Generic;

namespace project2
{

    /*
        The scanner takes in raw source code as a series of characters and groups it into
        a series of chunks we call tokens. These are meaningful "words" and "punctuation"
        that make up the language's grammar.
    */
    class Scanner {

        // String source to scan from (read in from file or console line)
        private readonly string source;

        // List of tokens that gets added to
        List<Token> tokens = new List<Token>();

        // Start index of current lexeme
        private int start = 0;

        // Current offset from start of file 
        private int current = 0;

        // Current line in text
        private int line = 1;

        /*  
        List of reserved Lox words that we need to check for when reading in identifiers 
        */
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
            {"while", TokenType.WHILE},
            {"whilenoout", TokenType.WHILENOOUT},
            {"fornoout", TokenType.FORNOOUT}
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
            char c = Advance(); // Gets the current character then adds 1 to current
            
            // Check all possible values of the char
            switch (c) {
                // call AddToken if we encounter a single one-character token
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
                // Must check the next char ahead for the below operators
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
                    if (Match('/')) { // Found a comment, advance to next line
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    } else { // Add slash token otherwise
                        AddToken(TokenType.SLASH);
                    }
                    break;
                // Skip over newlines and whitespace 
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                // Call appropriate methods for below multi-char lexemes
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

        /*
        If we reach an identifier (variable name), that is not in our dictionary, add it.
        Otherwise, if the word matches a type that is already existing such as "or", then just add that token to the list. 
        */
        private void Identifier() {
            while (IsAlphaNumeric(Peek())) {
                Advance();
            }

            // Check the substring and add it to our dictionary
            var text = source.Substring(start, current - start); 
            TokenType type;
            if (!keywords.ContainsKey(text)) { // Check if the lexeme is a preexisting Lox keyword. may need to change this somehow to check for a null value?
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

        /*
        Consume as many digits as we find for the integer part of the literal, then look for decimal followed by at least one digit. 
        First utilization of PeekNext() method to check two chars ahead. 
        */
        private void Number() {
            // While we haven't reached whitespace or decimal, advance
            while (IsDigit(Peek())) {
                Advance();
            }

            // If we reach a decimal while scanning the number then advance and continue consuming until no digits
            if (Peek() == '.' && IsDigit(PeekNext())) {
                // Consume the '.'
                Advance();
                while (IsDigit(Peek())) {
                    Advance();
                }
            }

            // Add a token with the literal being the substring of the number casted to double
            AddToken(TokenType.NUMBER, double.Parse(source.Substring(start, current - start)));
        }

        /*
        Consume characters until we reach another " character that ends the string.
        Gracefully handle running out of input before the end of a string. 
        */
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

            // Get the actual text of the string without quotes
            string value = source.Substring(start + 1, current - start - 2); // Subtract 2 to remove quotes

            // Add token to our list of tokens
            AddToken(TokenType.STRING, value);
        }
        

        /*
        Check if the char at current matches the passed in character, and return true if it does. False otherwise. 
        */
        private bool Match(char expected) {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;
            // Return true and increment if the character matches
            current ++;
            return true;
        }

        /*
        Returns the next char that will be returned (at current)
        */
        private char Peek() {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        /*
         Returns the char that will be returned after the next one if it exists
         */
        private char PeekNext() {
            if (current + 1 >= source.Length) {
                return '\0';
            }
            return source[current+1];
        }


        /*
        Return true if input char is a number, false otherwise
        */
        private bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }


        // Check if at the end of file (all chars have been consumed)
        private bool IsAtEnd() {
            return current >= source.Length;
        }

        // Advance current and return the old value
        private char Advance() {
            var prev = current;
            current++;
            return source[prev];
        }

        /*
        Add a token with the given type to the dictionary.
        Use when Token literal is not specified
        */
        private void AddToken(TokenType type) {
            AddToken(type, null);
        }


        /*
        Add a token with the given type and literal values to the dictionary.
        */
        private void AddToken(TokenType type, object literal) {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}