namespace project2
{
    public abstract classStmt
{
        public interface Visitor<T>
        {
            T visitBlockStmt(Block stmt);
            T visitExpressionStmt(Expression stmt);
            T visitIfStmt(If stmt);
            T visitPrintStmt(Print stmt);
            T visitVarStmt(Var stmt);
            T visitWhileStmt(While stmt);
        }

       static class Block: Stmt
        {
            Block(List<Stmt> statements) {
                this.statements = statements;
            }

          final List<Stmt> statements;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitBlockStmt(this);
            }
        }

       static class Expression: Stmt
        {
            Expression(Expr expression) {
                this.expression = expression;
            }

          final Expr expression;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitExpressionStmt(this);
            }
        }

       static class If: Stmt
        {
            If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
                this.condition = condition;
                this.Stmt = Stmt;
                this.Stmt = Stmt;
            }

          final Expr condition;
          final  Stmt thenBranch;
          final  Stmt elseBranch;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitIfStmt(this);
            }
        }

       static class Print: Stmt
        {
            Print(Expr expression) {
                this.expression = expression;
            }

          final Expr expression;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitPrintStmt(this);
            }
        }

       static class Var: Stmt
        {
            Var(Token name, Expr initializer) {
                this.name = name;
                this.Expr = Expr;
            }

          final Token name;
          final  Expr initializer;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitVarStmt(this);
            }
        }

       static class While: Stmt
        {
            While(Expr condition, Stmt body) {
                this.condition = condition;
                this.Stmt = Stmt;
            }

          final Expr condition;
          final  Stmt body;
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitWhileStmt(this);
            }
        }

    abstract <T> T accept(Visitor<T> visitor);
}
