using System.Collections.Generic;

namespace project2
{

    /*
    The parser consumes a flat input sequence like scanner, but reads in tokens instead of characters.
    The list of tokens is stored, and current is used to point to the next token to be parsed.
    The recursive descent parsing method is used, where the outermost grammar rules is worked first, followed by 
    nested subexpressions and eventually leaves of the syntax tree.
    */
    class Parser {
        
        // ParseError class inherits from System.Exception
        private class ParseError : System.Exception {}

        // List of tokens to be added to
        private readonly List<Token> tokens;

        // Current token to be read
        private int current = 0;

        // Constructor
        public Parser(List<Token> tokens) {
            this.tokens = tokens;
        }

        /*
        
        */
        private Expr Expression() {
            return Assignment();
        }

        private Stmt Declaration(){
            try{
                if(Match(TokenType.FUN)) return Function("function");
                if (Match(TokenType.VAR)) return VarDeclaration();
                return Statement();
            } catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }
        private Stmt Statement() {
            if (Match(TokenType.FOR)) return ForStatement();
            if(Match(TokenType.IF)) return IfStatement();
            if(Match(TokenType.PRINT)) return PrintStatement();
            if(Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if(Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        private Stmt ForStatement() {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt initializer;
            if (Match(TokenType.SEMICOLON)) {
                initializer = null;
            } else if (Match(TokenType.VAR)) {
                initializer = VarDeclaration();
            } else {
                initializer = ExpressionStatement();
            }

            Expr condition = null;
            if (!Check(TokenType.SEMICOLON)) {
                condition = Expression();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            Expr increment = null;
            if (!Check(TokenType.RIGHT_PAREN)) {
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");
            Stmt body = Statement();

            if (increment != null) {
                body = new Stmt.Block(
                    new List<Stmt> {body, new Stmt.Expression(increment)}                    
                );
            }

            if (condition == null) {
                condition = new Expr.Literal(true);
            }
            body = new Stmt.While(condition, body);

            if (initializer != null) {
                body = new Stmt.Block(
                    new List<Stmt> {initializer, body}
                );
            }
            return body;
        }

        private Stmt IfStatement() {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(TokenType.ELSE)) {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt PrintStatement(){
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt ReturnStatement() {
            Token keyword = Previous();
            Expr val = null;

            if(!Check(TokenType.SEMICOLON)){
                val = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(keyword,val);
        }

        private Stmt VarDeclaration() {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL)) {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt WhileStatement() {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            Stmt body = Statement();

            return new Stmt.While(condition, body);
        }

        private Stmt ExpressionStatement(){
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private Stmt.Function Function(string kind){
            Token name = Consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
            Token p = Consume(TokenType.LEFT_PAREN, "Expect '(' before parameter list");
            List<Token> parameters = new List<Token>();

            if(!Check(TokenType.RIGHT_PAREN)) {
                do{
                    if (parameters.Count >= 255){
                       Error(Peek(), "Can't have more than 255 parameters."); 
                    }

                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                }while (Match(TokenType.COMMA));

            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");
            Consume(TokenType.LEFT_BRACE, "Expect '{' before "+kind+ " body.");
            List<Stmt> body = Block();
            return new Stmt.Function(name,parameters,body);
        }

        private List<Stmt> Block(){
            List<Stmt> statements = new List<Stmt>();

            while(!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()){
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        /*
        Returns a new Expr 
        */
        private Expr Assignment(){
            Expr expr = Or();

            if (Match(TokenType.EQUAL)){
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable){
                    Token name = ((Expr.Variable)expr).name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or() {
            Expr expr = And();

            while (Match(TokenType.OR)) {
                Token oper = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, oper, right);
            }

            return expr;
        }

        private Expr And() {
            Expr expr = Equality();

            while (Match(TokenType.AND)) {
                Token oper = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, oper, right);
            }

            return expr;
        }

        /*
        Later chapter
        */
        public List<Stmt> Parse() {
            var statements = new List<Stmt>();
            while(!IsAtEnd()){
                statements.Add(Declaration());
            }

            return statements;
        }

        /*
        equality -> comparison ( ( "!=" | "==" ) comparison )* ;

        If we find != or ==, then operate on those with the operands to the left and right. 

        */
        private Expr Equality() {
            // First check for Comparison() since those have higher precedence in Lox. 
            Expr expr = Comparison();

            // Maps to ( ... )* loop --> don't exit until we see something that is not != or ==
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
                Token oper = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        /*
        comparison --> term ( ( ">" | ">=" | "<" | "<=" ) term )* ;

        If Equality() never found an equality operator, then it effectively returns a call to Comparison().
        Check for the symbols in the above grammar, and return a binary expression called on the two operands if found. 
        */
        private Expr Comparison() {
            // Check for additiion or subtraction operators first, which have higher precedence. 
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
                Token oper = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        /*
        Checks for Addition or Subtraction symbols, returning a binary expression on the operands if found. 
        Othwerise, call Factor() to handle multiplication or division.
        */
        private Expr Term() {
            // Call Factor() first, since mult / div have higher precedence
            Expr expr = Factor();

            // Check for - or + operations, and operate on them if found with the values to left and right 
            while (Match(TokenType.MINUS, TokenType.PLUS)) {
                Token oper = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        /*
        Checks for multiplication or division symbols, returning a binary expression on the operands if found.
        */
        private Expr Factor() {
            // Check for Unary() first, since it has higher precedence
            Expr expr = Unary();

            // Check for / or * operators, and operate on them with the values to the left and right if found
            while (Match(TokenType.SLASH, TokenType.STAR)) {
                Token oper = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        /*
        Checks for ! or - symbols in front of a token, returning a Unary expression on the operand if found. 
        */
        private Expr Unary() {
            // BANG and MINUS can both negate a single numeric value
            if (Match(TokenType.BANG, TokenType.MINUS)) {
                Token oper = Previous();
                Expr right = Unary();
                return new Expr.Unary(oper, right);
            }

            return Call();
        }

        private Expr FinishCall(Expr callee){
            List<Expr> arguments = new List<Expr>();
            if(!Check(TokenType.RIGHT_PAREN)){
                do {
                    if (arguments.Count >= 255){
                        Error(Peek(), "Can't have more than 255 arguments.");
                    }
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));
            }

            Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);
        }

        private Expr Call(){
            Expr expr = Primary();

            while(true){
                if(Match(TokenType.LEFT_PAREN)){
                    expr = FinishCall(expr);
                } else{
                    break;
                }
            }

            return expr;
        }

        /*
        Highest level of precedence.
        primary --> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" ;

        Return a new Expr type corresponding to the TokenType received from Match().

        */
        private Expr Primary() {
            // Most primarys are straightforward, and can be handled as so
            if (Match(TokenType.FALSE)) {
                return new Expr.Literal(false);
            }
            if (Match(TokenType.TRUE)) {
                return new Expr.Literal(true);
            }
            if (Match(TokenType.NIL)) {
                return new Expr.Literal(null);
            }

            if (Match(TokenType.NUMBER, TokenType.STRING)) {
                return new Expr.Literal(Previous().literal);
            }

            if (Match(TokenType.IDENTIFIER)){
                return new Expr.Variable(Previous());
            }
            // Parentheses are a bit more complicated, because we must parse the expression inside, and if we don't find a closing ), throw error. 
            if (Match(TokenType.LEFT_PAREN)) {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            // Sitting on a token that cannot start an expression, so handle that error 
            throw Error(Peek(), "Expect expression.");
        }

        /*
        Check to see if the current token is any of the given types.
        If so, consume the token and return true. Otherwise, return false and leave the token alone.

        */
        private bool Match(params TokenType[] types) {
            foreach (TokenType type in types) {
                if (Check(type)) {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        /*
        Check if the next token is the expected type, and consume it if so.
        Otherwise, throw an error. 
        */
        private Token Consume(TokenType type, string message) {
            if (Check(type)) {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        /*
        Returns true if the current token is of a given type.
        Unlike Match(), does not consume the token.
        */
        private bool Check(TokenType type) {

            // If at the end of the file, return false
            if (IsAtEnd()) {
                return false;
            }

            // Otherwise, check if next character matches the type of the argument
            return Peek().type == type;
        }


        /*
        Consume the current token and return it, similar to how scanner crawls through characters. 
        */
        private Token Advance() {
            // If not at the end of the EOF token, advance to next
            if (!IsAtEnd()) {
                current ++;
            }
            // Return the token we just advanced from
            return Previous();
        }


        /*
        Returns true if the next token's type if EOF
        */
        private bool IsAtEnd() {
            return Peek().type == TokenType.EOF;
        }

        /*
        Return the next token to be consumed (the one at current)
        */
        private Token Peek() {
            return tokens[current];
        }

        /*
        Return the last token that has been consumed (current -1)
        */
        private Token Previous() {
            return tokens[current-1];
        }

        /*
        Call the Lox class' error handler, and return the ParseError
        */  
        private ParseError Error(Token token, string message) {
            Lox.Error(token.line, message);
            return new ParseError();
        }

        /*
        This method discards tokens until it thinks it has found a statement boundary.
        Most statements start with a keyword - for, if return, var, etc.
        If the next token is any of those, we are probably about to start a statement. 
        */
        private void Synchronize() {
            Advance();

            // Read for semicolons as long as we're not at EOF
            while (!IsAtEnd()) {
                if (Previous().type == TokenType.SEMICOLON) {
                    return;
                }

                // If next token is a keyword, we can return (ready to parse next statement)
                switch(Peek().type) {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                // Advance to start of next statement 
                Advance();
            }
        }



    }

}