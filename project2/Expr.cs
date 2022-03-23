
namespace project2
{
    abstract class Expr
    {
        public class Binary : Expr
        {
            Expr Left;
            Token Op;
            Expr Right;
            public Binary(Expr left, Token op, Expr right)
            {
                this.Left = left;
                this.Op = op;
                this.Right = right;
            }
        }

    }
}