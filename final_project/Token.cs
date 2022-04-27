using System;

namespace project2 {

    /*
    Token object used to represent tokens when scanning the Lox code 
    */
    public class Token {

        // Type of token
        public readonly TokenType type;

        // Lexeme "string"
        public readonly string lexeme;

        // Literal "object"
        public readonly Object literal;

        // Line token appears on
        public readonly int line;

        /*
        Constructor
        */
        public Token(TokenType type, string lexeme, Object literal, int line) {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        /*
        Returns the string version of the token
        */
        public override string ToString() {
            return type + " " + lexeme + " " + literal;
        }

    }

}
