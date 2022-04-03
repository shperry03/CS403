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
            public T VisitIfStmt(If stmt);
            public T VisitWhileStmt(While stmt);
        }

        public class If: Stmt
        {
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            public Expr condition {get;}
            public Stmt thenBranch {get;}
            public Stmt elseBranch {get;}
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
        }

        public class While: Stmt
        {
            public While(Expr condition, Stmt body) {
                this.condition = condition;
                this.body = body;
            }

          public Expr condition;
          public Stmt body;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
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
