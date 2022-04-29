using System.Collections.Generic;

namespace project2
{
    public abstract class Expr
    {
        public interface Visitor<T>
        {
            /*
                The interface for Expr class that allows us to implement these functions 
                in the interpreter class. All expression classes must inherit this class
                and these funcitons.
            */
            public T VisitAssignExpr(Assign expr);
            public T VisitBinaryExpr(Binary expr);
            public T VisitCallExpr(Call expr);
            public T VisitGroupingExpr(Grouping expr);
            public T VisitLiteralExpr(Literal expr);
            public T VisitLogicalExpr(Logical expr);
            public T VisitUnaryExpr(Unary expr);
            public T VisitVariableExpr(Variable expr);
        }

        /*
            The Assign subclass of Expression.
            Allows us to implement an assignment and when called:

            create a variable : name
            assign a value to variable : value
        */
        public class Assign: Expr
        {
            public Assign(Token name, Expr value) {
                this.name = name;
                this.value = value;
            }

            public Token name;
            public Expr value;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }
        /*
            The Binary subclass of Expression.
            Allows us to account for operators and setting arithmetic expressions.

            1 + 2
            lef = 1
            op = +
            right = 2

            Breaking a 2 argument arithmetic expression into individual parts.
        */
        public class Binary: Expr
        {
            
            public Binary(Expr left, Token op, Expr right) {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public Expr left;
            public Token op;
            public Expr right;

            public override string ToString()
            {
                return " (" + left + " " + op.lexeme + " " + right + ") ";
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        /*
            The Call subclass for expression.
            This allows us to call functions in lox code.
            
            Passing paramters and calling the function is possible here.
        */
        public class Call: Expr
        {
            public Call(Expr callee, Token paren, List<Expr> arguments) {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            public Expr callee;
            public Token paren;
            public List<Expr> arguments;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        /*
            The Grouping subclass for Expression.
            Allows us to group expressions using parentheses.
        */
        public class Grouping: Expr
        {
            public Grouping(Expr expression) {
                this.expression = expression;
            }

            public Expr expression;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        /*
            The Literal subclass for Expression.

            This allows us to have literal values like Numbers, Strings, Booleans, and nil.
            Accounts for these literal expressions.
        */
        public class Literal: Expr
        {
            public Literal(object value) {
                this.value = value;
            }

            public object value;
            public override string ToString()
            {
                return value.ToString();
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        /*
            The Unary subclass for Expression.

            Accounts for operators on a single value.

            Allows ! and - for different values of expressions and numbers.
        */
        public class Unary: Expr
        {
            public Unary(Token op, Expr right) {
                this.op = op;
                this.right = right;
            }

            public Token op;
            public Expr right;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        /*
            The Logical subclass for Expression.

            Implements logical expressions in our lox code and interpreter.
        */

        public class Logical: Expr
        {
            public Logical(Expr left, Token oper, Expr right) {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }

          public Expr left;
          public  Token oper;
          public  Expr right;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }


        /*
            The variable subclass for Expression.

            Allows us to have a variable expresssion.

            Declaring a variable name and using it expressions.
        */
        public class Variable: Expr
        {
            public Variable(Token name) {
                this.name = name;
            }

            public Token name;

            public override string ToString()
            {
                return name.lexeme;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

        public abstract T Accept<T>(Visitor<T> visitor);
    }
}
