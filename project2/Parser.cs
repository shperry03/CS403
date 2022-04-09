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
        Expression function just calls the assignment function and return the value
        */
        private Expr Expression() {
            return Assignment();
        }

        /*
        Declaration function called to parse statements
        */
        private Stmt Declaration(){
            try{
                // If we have a function return Function()
                if (Match(TokenType.FUN)) return Function("function");
                // If we have a var, Call VarDeclaration function
                if (Match(TokenType.VAR)) return VarDeclaration();
                // Else just handle statement
                return Statement();
            } catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }
        /*
        Determines whihc statement rule matches the token and returns the correct rule
        */
        private Stmt Statement() {
            // Go through and see which token matches for statements
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        /*
        Performs 'desugaring' on the for statement, essentially turning it into a Lox while statement
        */
        private Stmt ForStatement() {
            // If no ( after for, throw error
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            // Grab the for loop's initializer
            Stmt initializer;
            if (Match(TokenType.SEMICOLON)) { // Initializer has been omitted
                initializer = null;
            } else if (Match(TokenType.VAR)) { // Initializer is variable declaration
                initializer = VarDeclaration();
            } else { // Otherwise, initializer must be an expression
                initializer = ExpressionStatement();
            }

            // Grab the for loop's condition
            Expr condition = null;
            if (!Check(TokenType.SEMICOLON)) { // If next token is not a semicolon, get the expression
                condition = Expression();
            }
            // Throw an error if no semicolon
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            // Grab the for loop's increment value
            Expr increment = null;
            if (!Check(TokenType.RIGHT_PAREN)) { // If no right parentheses, we have an expression
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses."); // If no right parentheses, throw error

            // Create the Statement representing the body
            Stmt body = Statement();

            // If there is an increment, it must execute after the body in each iteration.
            if (increment != null) {
                // Replace the body with a block containing original body followed by an expression that evaluates the increment
                body = new Stmt.Block(
                    new List<Stmt> {body, new Stmt.Expression(increment)}                    
                );
            }
            // If there is no condition, we will have an infinite loop
            if (condition == null) {
                condition = new Expr.Literal(true);
            }
            // Replace body with a while statement, passing in the condition and body
            body = new Stmt.While(condition, body);

            // If there is an initializer, it must run once before the entire loop.
            if (initializer != null) {
                // Replace the whole statement with a block that runs the initializer and then executes the loop
                body = new Stmt.Block(
                    new List<Stmt> {initializer, body}
                );
            }
            return body;
        }

        /*
        Handles if statements in Lox.
        Saves various variables locally, and then passes them to a Stmt condition that is returned.
        */
        private Stmt IfStatement() {
            // If no ( after if, we have an error
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            // Get the expression that is inside the ( )
            Expr condition = Expression();
            // If no ) after condition, throw an error
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            // thenBranch represents what we do if the condition is true
            Stmt thenBranch = Statement();
            Stmt elseBranch = null; 
            // Check for an else branch, and create a statement variable for it if it exists
            if (Match(TokenType.ELSE)) {
                elseBranch = Statement();
            }

            // Create and return a new Stmt.If
            return new Stmt.If(condition, thenBranch, elseBranch);
        }
        
        /*
        Handles Print statements in Lox.
        Parses the expressions after a print statement until ; is hit.
        */
        private Stmt PrintStatement(){
            Expr value = Expression();
            // Consume until semicolon token
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            // returns new Stmt.Print with the value associated.
            return new Stmt.Print(value);
        }

        /*
        Handles return statements in lox
        Parses the return in a given statement
        */

        private Stmt ReturnStatement() {
            // Set the keyword and value
            Token keyword = Previous();
            Expr val = null;

            // Make the value the correct expression before the ;
            if(!Check(TokenType.SEMICOLON)){
                val = Expression();
            }
            // check for semicolon and consume it
            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            // return the new return statement parsed
            return new Stmt.Return(keyword,val);
        }

        /*
        Called in the Declaration Function up top.

        If a variable is parsed, this handles that
        */
        private Stmt VarDeclaration() {
            // Consume the identifier and return the function name
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            // If they initialize it, set the value to the correct value Expression()
            if (Match(TokenType.EQUAL)) {
                initializer = Expression();
            }
            // Else declares variable and checks for syntax
            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            // Returns a variable with the name and Value
            return new Stmt.Var(name, initializer);
        }

        /*
        Creates a Lox While statement from the condition inside parentheses and the body
        */
        private Stmt WhileStatement() {
            // If no ( after while, throws error
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            // Grab the condition to check for inside ( )
            Expr condition = Expression();
            // If no ) after condition, throw error
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            // Grab body that will be executed
            Stmt body = Statement();

            // Return Lox while statement
            return new Stmt.While(condition, body);
        }

        /*
        Handles expression statements in Lox.
        */
        private Stmt ExpressionStatement(){
            Expr expr = Expression();
            // Consume until semicolon token is found.
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            // returns a new Stmt.Expresssion with expr value.
            return new Stmt.Expression(expr);
        }

        /*
        Handles parsing function statements in lox
        Goes through the statement and breaks it down into expressions. 
        */

        private Stmt.Function Function(string kind){
            // Make sure the function is declared correctly
            Token name = Consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
            Token p = Consume(TokenType.LEFT_PAREN, "Expect '(' before parameter list");
            // create a new list to store parameters
            List<Token> parameters = new List<Token>();

            // Add the parameters into the list
            if(!Check(TokenType.RIGHT_PAREN)) {
                do{
                    if (parameters.Count >= 255){
                       Error(Peek(), "Can't have more than 255 parameters."); 
                    }

                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                }while (Match(TokenType.COMMA));

            }
            // Finish parsing braces and parentheses
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");
            Consume(TokenType.LEFT_BRACE, "Expect '{' before "+kind+ " body.");
            // Create a new block for the body of the function
            List<Stmt> body = Block();
            // return the function declaration
            return new Stmt.Function(name,parameters,body);
        }

        /*
        BLock fucntion
        ALlows us to run code in blocks of { }
        */
        private List<Stmt> Block(){
            // Create a new list of statements
            List<Stmt> statements = new List<Stmt>();

            while(!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()){
                // Add the statements until a right brace or end of file
                statements.Add(Declaration());
            }
            // no right brace found checker and parser
            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            // return the block of statements in List format for execution
            return statements;
        }

        /*
        Recursively parses the right hadn sife of an assignment expression
        */
        private Expr Assignment(){
            Expr expr = Or();

            if (Match(TokenType.EQUAL)){
                // Make sure the assignment is valid
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable){
                    // creates the name given to it
                    Token name = ((Expr.Variable)expr).name;
                    // returns the expr assignment
                    return new Expr.Assign(name, value);
                }
                //if gets here, then it wasn't a variable
                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        /*
        Parses a series of or expressions
        */
        private Expr Or() {
            // First check for and (higher precedence)
            Expr expr = And();

            // While we have an or condition, create a logical expression based around the operands
            while (Match(TokenType.OR)) {
                Token oper = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, oper, right);
            }

            return expr;
        }

        /*
        Parses a series of and expressions
        */
        private Expr And() {
            // First check for equality (higher precedence)
            Expr expr = Equality();

            // While we have an and condition, create logical expression based around the operands
            while (Match(TokenType.AND)) {
                Token oper = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, oper, right);
            }

            return expr;
        }

        /*
        Yhid parses a series of statements until it hits end of input
        */
        public List<Stmt> Parse() {
            var statements = new List<Stmt>();
            // While there is still input, parse statement
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

        /*
        Finish call that actually parses the rest of the call
        */
        private Expr FinishCall(Expr callee){
            // Create the arg list
            List<Expr> arguments = new List<Expr>();
            if(!Check(TokenType.RIGHT_PAREN)){
                do {
                    // Check for too many args
                    if (arguments.Count >= 255){
                        Error(Peek(), "Can't have more than 255 arguments.");
                    }
                    // add every argument to new list
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));
            }
            //when hit the final parenthesize check to make sure its there
            Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

            //Returnt The call fucntion on the expr and args
            return new Expr.Call(callee, paren, arguments);
        }

        /*
        Call funciton for the parser
        */
        private Expr Call(){
            // parse the primary expr
            Expr expr = Primary();

            while(true){
                // When hits the left paren, call the FInishCall on the rest of the expr
                if(Match(TokenType.LEFT_PAREN)){
                    expr = FinishCall(expr);
                } else{
                    break;
                }
            }
            // return the expression
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