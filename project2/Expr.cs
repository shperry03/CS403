

namespace project2
{
    public abstract class Expr
    {
        public interface Visitor<T>
        {
            T visitBinaryExpr(Binary expr);
            T visitGroupingExpr(Grouping expr);
            T visitLiteralExpr(Literal expr);
            T visitUnaryExpr(Unary expr);
        }

        public class Binary: Expr
        {
            public Binary(Expr left, Token op, Expr right) {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            Expr left;
            Token op;
            Expr right;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitBinaryExpr(this);
            }
        }

        public class Grouping: Expr
        {
            public Grouping(Expr expression) {
                this.expression = expression;
            }

            Expr expression;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitGroupingExpr(this);
            }
        }

        public class Literal: Expr
        {
            public Literal(object value) {
                this.value = value;
            }

            object value;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }
        }

        public class Unary: Expr
        {
            public Unary(Token op, Expr right) {
                this.op = op;
                this.right = right;
            }

            Token op;
            Expr right;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }
        }
        public abstract T Accept<T>(Visitor<T> visitor);

    
    }
}
