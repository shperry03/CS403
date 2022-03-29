using System;
using System.Collections.Generic;

namespace project2 {
    class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evaluate(expr.right);

            switch (expr.op.type) {
                case TokenType.MINUS:
                    CheckNumberOperand(expr.op, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
            }

            // unreachable
            return null;
        }

        private void CheckNumberOperand(Token op, object operand) {
            if (operand.GetType() == typeof(double)) {
                return;
            }
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object left, object right) {
            if (left.GetType() == typeof(double) && right.GetType() == typeof(double)) {
                return;
            }

            throw new RuntimeError(op, "Operands must be numbers.");
        }

        private bool IsTruthy(object obj) {
            if (obj == null) {
                return false;
            }

            if (obj.GetType() == typeof(bool)) {
                return (bool)obj;
            }
            return true;
        }

        private bool IsEqual(object a, object b) {
            if (a == null && b == null) {
                return true;
            }
            if (a == null) {
                return false;
            }
            
            return a.Equals(b);
        }

        private string Stringify(object obj) {
            if (obj == null) {
                return "nil";
            }

            if (obj.GetType() == typeof(double)) {
                string text = obj.ToString();
                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }

        private object Evaluate(Expr expr) {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt){
            stmt.Accept(this);
        }

        public object VisitExpressionStmt(Stmt.Expression stmt){
            Evaluate(stmt.expression);
            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt){
            object value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);

            switch (expr.op.type) {
                case TokenType.GREATER:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left.GetType() == typeof(double) && right.GetType() == typeof(double)) { // may need to be is?
                        return (double)left + (double)right;
                    }
                    if (left.GetType() == typeof(string) && right.GetType() == typeof(double)) {
                        return (string)left + (string) right;
                    }
                    
                    throw new RuntimeError(expr.op, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left * (double)right;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            // Unreachable
            return null;
        }

        public void interpret(List<Stmt> statements) {
            try {
                foreach (Stmt statement in statements){
                    Execute(statement);
                }
            }
            catch (RuntimeError error) {
                Lox.RuntimeError(error);
            }
        }
    }
}