using System.Collections.Generic;

namespace project2
{
    /*
    The Stmt class represents an entire statement in Lox, and provides the interface with functions necessary
    to visit and execute a statement. 
    */
    public abstract class Stmt
    {
        public interface Visitor<T>
        {
            public T VisitBlockStmt(Block stmt);
            public T VisitExpressionStmt(Expression stmt);
            public T VisitFunctionStmt(Function stmt);
            public T VisitIfStmt(If stmt);
            public T VisitPrintStmt(Print stmt);
            public T VisitReturnStmt(Return stmt);
            public T VisitVarStmt(Var stmt);
            public T VisitWhileStmt(While stmt);
        }

        // Block class from statement class
        public class Block: Stmt
        {
            // Constructor for a block of code
            public Block(List<Stmt> statements) {
                this.statements = statements;
            }
            // List of the body statements in {}
            public List<Stmt> statements;
            // Accept function for the visitor
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
        }

        // Expression subclass of Statement
        public class Expression: Stmt
        {
            // Constructor for a new expression
            public Expression(Expr expression) {
                this.expression = expression;
            }
            // stores the expression object value
            public Expr expression;
            // Accept function for visitor
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }


        /*
        Function subclass of statment 
        allows us to handle functions in Lox
        */
        public class Function: Stmt
        {
            // Constructor for a function
            public Function(Token name, List<Token> param, List<Stmt> body) {
                this.name = name;
                this.param = param;
                this.body = body;
            }
            //Function Name
            public Token name;
            // List of parameters
            public List<Token> param;
            // List of statements that give functionality
            public List<Stmt> body;
            // Accept statement for the visitor
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
        }

        /*
        If subclass of Statement
        Allows us to declare If statements as objects
        */
        public class If: Stmt
        {
            // Constructor for if statement
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }
            // Condition of the If
            public Expr condition {get;}
            // then branch if exists
            public Stmt thenBranch {get;}
            // else branch if exists
            public Stmt elseBranch {get;}
            // Accept statement for the visitor
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
        }

        /*
        The print subclass of Statment.

        Creates a separate class for the handling of print statements in lox.
        */
        public class Print: Stmt
        {
            // Constructor
            public Print(Expr expression) {
                this.expression = expression;
            }

            // Expression value 
            public Expr expression;
            // Accept function written for Visitor 
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        /*
        Return subclass of statements
        Creates a class for return statements in Lox
        */
        public class Return: Stmt
        {
            // Constructor
            public Return(Token keyword, Expr value) {
                this.keyword = keyword;
                this.value = value;
            }
            // Keyword of the Return
            public Token keyword;
            // Value of the return
            public Expr value;
            // Accept statement for the visitor
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
        }

        /*
        The var subclass of Statement

        Creates a separate class for handling var statements
        */
        public class Var: Stmt
        {   
            // Constructor
            public Var(Token name, Expr initializer) {
                this.name = name;
                this.initializer = initializer;
            }
            // Variable name
            public Token name;
            // Variable val of type expr 
            public Expr initializer;
            //Accept funciton for visitor
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

        /*
        While subclass of Statement
        Creates a class to handle while loops
        */
        public class While: Stmt
        {   
            //Constructor
            public While(Expr condition, Stmt body) {
                this.condition = condition;
                this.body = body;
            }
            // Condition on which to loop
            public Expr condition;
            // Functionality of the while statement
            public Stmt body;
            // Accept statement for the visitor
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }

        // Abstract accept function for the class
        public abstract T Accept<T>(Visitor<T> visitor);
    }
}
