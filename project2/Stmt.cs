using System.Collections.Generic;

namespace project2
{
    public abstract class Stmt
    {
        public interface Visitor<T>
        {
            public T VisitBlockStmt(Block stmt);
            public T VisitExpressionStmt(Expression stmt);
            public T VisitPrintStmt(Print stmt);
            public T VisitVarStmt(Var stmt);
        }

        public class Block: Stmt
        {
            public Block(List<Stmt> statements) {
                this.statements = statements;
            }

            public List<Stmt> statements;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
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

        public class Var: Stmt
        {
            public Var(Token name, Expr initializer) {
                this.name = name;
                this.initializer = initializer;
            }

            public Token name;
            public Expr initializer;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

        public abstract T Accept<T>(Visitor<T> visitor);
    }
}
