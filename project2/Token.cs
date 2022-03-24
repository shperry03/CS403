using System;

namespace project2 {

    /*
        Token class used to represent Tokens 
    */
    public class Token {
        public readonly TokenType type;
        public readonly string lexeme;
        public readonly Object literal;
        public readonly int line;

        public Token(TokenType type, string lexeme, Object literal, int line) {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString() {
            return type + " " + lexeme + " " + literal;
        }

    }

}
