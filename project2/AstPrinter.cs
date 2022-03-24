using System.Text;
using System;

namespace project2
{
    public class AstPrinter : Expr.Visitor<string>
    {
        string print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return parenthesize(expr.op.lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.value == null) return "nil";
            return expr.value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return parenthesize(expr.op.lexeme, expr.right);
        }

        private string parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(").Append(name);
            foreach(var expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append(")");
            return sb.ToString();
        }

        public void Main(string[] args)
        {
            Expr expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenType.STAR, "*",null,1),
                new Expr.Grouping(
                    new Expr.Literal(45.67)
                )
            );

            Console.WriteLine(new AstPrinter().print(expression));
        }
    }
}
