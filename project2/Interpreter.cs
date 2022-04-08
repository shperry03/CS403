using System;
using System.Collections.Generic;


namespace project2 {

    /*
    Implements the Visitor pattern. Visit methods must return objects.

    */
    public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        public static Environment globals = new Environment();

        public Environment environment = globals;

        public Interpreter(){
            
        }

        /*
        Evaluator for expressions inside parentheses. 
        Uses the Evaluate helper method to recursively evaluate subexpression and retur nit.
        */
        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        /*
        Simply return the runtime value of the literal expression
        */
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

        /*
        Unary expressions have single subexpressions that we must evaluate first.
        The unary expression itself then does a little work afterwards.
        */
        public object VisitUnaryExpr(Expr.Unary expr)
        {   
            // Evaluate the operand expression first
            object right = Evaluate(expr.right);

            // Apply the unary operator to the result of evaluating the operand
            switch (expr.op.type) {
                case TokenType.MINUS: // Check if number is a double, then negate it if so
                    CheckNumberOperand(expr.op, right);
                    return -(double)right;
                case TokenType.BANG: // Check if number is logical, then negate it if so
                    return !IsTruthy(right);
            }

            // unreachable
            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr) {
            return environment.Get(expr.name);
        }

        /*
        Throws an error if the operand to check is not a number
        */
        private void CheckNumberOperand(Token op, object operand) {
            if (operand is double) {
                return;
            }
            throw new RuntimeError(op, "Operand must be a number.");
        }

        /*
        Checks if both operands are numbers, and throw an error if not. 
        */
        private void CheckNumberOperands(Token op, object left, object right) {
            if (left is double && right is double) {
                return;
            }

            throw new RuntimeError(op, "Operands must be numbers.");
        }

        /*
        In Lox, false and nil are falsey, and everything else is truthy. 
        This method takes in an object and returns true or false based on these rules. 
        */
        private bool IsTruthy(object obj) {
            // If object is nil, return false
            if (obj == null) {
                return false;
            }
            // If object is boolean, return its value
            if (obj.GetType() == typeof(bool)) {
                return (bool)obj;
            }

            // Otherwise, return true
            return true;
        }

        /*
        Implements Lox's version of equality between objects. 
        */
        private bool IsEqual(object a, object b) {
            // If both objects are nil, return true
            if (a == null && b == null) {
                return true;
            }
            // Otherwise if first object is nil, return false
            if (a == null) {
                return false;
            }
            // Otherwise, simply check for C# equality
            return a.Equals(b);
        }

        /*
        Convert the object to a string, whether it is a number or other type of object.
        */
        private string Stringify(object obj) {
            // If object is null, return nil string
            if (obj == null) {
                return "nil";
            }
            
            // If object is double, convert it to string and drop the .0 suffix if it exists
            if (obj is double) {
                string text = obj.ToString();
                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            
            // Otherwise, simply convert using C# ToString()
            return obj.ToString();
        }

        /*
        Simply send the expression back to the interpreter's visitor implementation, which visits a logical expression.
        */
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

        /*
        Handles and binary expressions (comparisons, simple math operations).
        */
        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);

            switch (expr.op.type) {
                case TokenType.GREATER: // Check that both vals are numbers, and return true if left is > right
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL: // Check that both vals are numbers, and return true if left is >= right
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS: // Check that both vals are numbers, andr eturn true if left < right
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL: // Check that both vals are numbers, and return true if left <= right
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS: // Check that both vals are numbers, and return left - right
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS: // Check whether vals are numbers or strings, and perform the addition operation on them 
                    if (left is double && right is double) { 
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string) {
                        return (string)left + (string) right;
                    }
                    // If operands are not both numbers or strings, throw RuntimeError
                    throw new RuntimeError(expr.op, "Operands must be two numbers or two strings.");
                case TokenType.SLASH: // Check if both vals are numbers, and return left / right
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR: // Check if both vals are numbers and return left * right
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left * (double)right;
                case TokenType.BANG_EQUAL: // Return true if left operand != right operand
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: // Return true if left operand == right operand
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

        /*
        TODO
        */
        public void Interpret(List<Stmt> statements) {
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