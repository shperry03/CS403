namespace project2
{
    public abstract class Stmt
    {
        public interface Visitor<T>
        {
            public T VisitExpressionStmt(Expression stmt);
            public T VisitPrintStmt(Print stmt);
        }

        public class Expression: Stmt
        {
            public Expression(Expr expression) {
                this.expression = expression;
            }

            public Expr expression;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        public class Print: Stmt
        {
            public Print(Expr expression) {
                this.expression = expression;
            }

            public Expr expression;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        public abstract T Accept<T>(Visitor<T> visitor);
    }
}
