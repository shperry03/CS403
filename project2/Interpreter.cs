using System;
using System.Collections.Generic;


namespace project2 {
    public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        public static Environment globals = new Environment();

        public Environment environment = globals;

        public Interpreter(){
            
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object VisitLogicalExpr(Expr.Logical expr) {
            Object left = Evaluate(expr.left);

            if (expr.oper.type == TokenType.OR) {
                if (IsTruthy(left)) {
                    return left;
                }
            } else {
                if (!IsTruthy(left)) {
                    return left;
                }
            }

            return Evaluate(expr.right);
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

        public object VisitVariableExpr(Expr.Variable expr) {
            return environment.Get(expr.name);
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

        public void ExecuteBlock(List<Stmt> statements, Environment environment){
            Environment previous = this.environment;

            try{
                this.environment = environment;

                foreach (Stmt statement in statements){
                    Execute(statement);
                }
            } finally {
                this.environment = previous;
            }
        }

        public object VisitBlockStmt(Stmt.Block stmt){
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        public object VisitExpressionStmt(Stmt.Expression stmt){
            Evaluate(stmt.expression);
            return null;
        }

        public object VisitFunctionStmt(Stmt.Function stmt){
            LoxFunction function = new LoxFunction(stmt);
            environment.Define(stmt.name.lexeme, function);
            return null;
        }

        public object VisitIfStmt(Stmt.If stmt) { // override?
            if (IsTruthy(Evaluate(stmt.condition))) {
                Execute(stmt.thenBranch);
            } else if (stmt.elseBranch != null) {
                Execute(stmt.elseBranch);
            }

            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt){
            object value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object VisitReturnStmt(Stmt.Return stmt){
            object val = null;
            if(stmt.value != null) val= Evaluate(stmt.value);
            throw new Return(val);
        }

        public object VisitVarStmt(Stmt.Var stmt){
            object value = null;
            if (stmt.initializer != null){
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.lexeme, value);

            return null;
        }

        public object VisitWhileStmt(Stmt.While stmt) {
            while (IsTruthy(Evaluate(stmt.condition))) {
                Execute(stmt.body);
            }

            return null;
        }

        public object VisitAssignExpr(Expr.Assign expr){
            object value = Evaluate(expr.value);
            environment.Assign(expr.name, value);
            return value;
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

        public object VisitCallExpr(Expr.Call expr) {
            object callee = Evaluate(expr.callee);

            List<object> arguments = new List<object>();

            foreach (Expr argument in expr.arguments){
                arguments.Add(Evaluate(argument));
            }

            if(!(callee is LoxCallable)) {
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            }

            LoxCallable function = (LoxCallable) callee;
            if(arguments.Count != function.Arity()){
                throw new RuntimeError(expr.paren, "Expected " + function.Arity() + " arguments but got " + arguments.Count + ".");
            }
            return function.Call(this, arguments);
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