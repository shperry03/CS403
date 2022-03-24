

namespace project2
{
    public abstract class Expr
    {
        public interface Visitor<T>
        {
            public T VisitBinaryExpr(Binary expr);
            public T VisitGroupingExpr(Grouping expr);
            public T VisitLiteralExpr(Literal expr);
            public T VisitUnaryExpr(Unary expr);
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
        public abstract T Accept<T>(Visitor<T> visitor);

    
    }
}
