using System.Collections.Generic;

namespace project2
{
    public abstract class Expr
    {
        public interface Visitor<T>
        {
            public T VisitAssignExpr(Assign expr);
            public T VisitBinaryExpr(Binary expr);
            public T VisitCallExpr(Call expr);
            public T VisitGroupingExpr(Grouping expr);
            public T VisitLiteralExpr(Literal expr);
            public T VisitLogicalExpr(Logical expr);
            public T VisitUnaryExpr(Unary expr);
            public T VisitVariableExpr(Variable expr);
        }

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
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }


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

        public class Literal: Expr
        {
            public Literal(object value) {
                this.value = value;
            }

            public object value;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

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

        public class Variable: Expr
        {
            public Variable(Token name) {
                this.name = name;
            }

            public Token name;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

        public abstract T Accept<T>(Visitor<T> visitor);
    }
}
