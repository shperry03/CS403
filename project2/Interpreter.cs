using System;
using System.Collections.Generic;


namespace project2 {

    /*
    Implements the Visitor pattern. Visit methods must return objects.

    */
    public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        // Declare environmetn for the variables
        public static Environment globals = new Environment();
        // assign Environemnt to the new created environment
        public Environment environment = globals;

        // Interpreter Constructor
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

        /*  
        Evaluates a logical expression by first evaluating the left side, then the right if AND
        */
        public object VisitLogicalExpr(Expr.Logical expr) {
            // Evaluate left expression
            Object left = Evaluate(expr.left);

            // If logical is OR, then return left if its truthy
            if (expr.oper.type == TokenType.OR) {
                if (IsTruthy(left)) {
                    return left; // Can return this instead of true because values are truthy in Lox
                }
            } else { // AND checker
                if (!IsTruthy(left)) {
                    return left; // Can return this instead of false because non-truthy values are falsey in Lox
                }
            }

            // Have to check the right side 
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

        /*
        Visitor function for variable expressions
        Just call get on the variable given and return it from the current environment
        */
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

        /*
        Sends a statement to the interpreter's visitor, essentially the statement equivalent of Evalutate()
        */
        private void Execute(Stmt stmt){
            stmt.Accept(this);
        }

        /*
        Execute block statement
        Allows code to be executed in blocks
        */
        public void ExecuteBlock(List<Stmt> statements, Environment environment){
            // Set the previous environment as the current one
            Environment previous = this.environment;

            try{
                // Assign new environemtn to the block
                this.environment = environment;
                // Execute all the statements in the cuurrent environment
                foreach (Stmt statement in statements){
                    Execute(statement);
                }
            } finally {
                //reset the old environment to be current
                this.environment = previous;
            }
        }

        // Visitor function for a Block statment
        public object VisitBlockStmt(Stmt.Block stmt){
            // Passes in the block list of statements and creates a new environment
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        /*
        Visit method for expression statements.

        */
        public object VisitExpressionStmt(Stmt.Expression stmt){
            // Evaluate on the expression passed in
            Evaluate(stmt.expression);
            // return null because we just want to evaluate the statement. 
            return null;
        }

        // Visitor function for a Function statement
        public object VisitFunctionStmt(Stmt.Function stmt){
            // Construct a new loxFunction 
            LoxFunction function = new LoxFunction(stmt);
            // Define the function with the name and LoxFunction object
            environment.Define(stmt.name.lexeme, function);
            return null;
        }

        /*
        Evaluates the condition, executing the thenBranch if true, otherwise execute elseBranch
        */
        public object VisitIfStmt(Stmt.If stmt) { // override?
            if (IsTruthy(Evaluate(stmt.condition))) {
                Execute(stmt.thenBranch);
            } else if (stmt.elseBranch != null) {
                Execute(stmt.elseBranch);
            }

            return null;
        }

        /*
        Visit method for print statments in lox.
        */
        public object VisitPrintStmt(Stmt.Print stmt){
            // Creates a new value from the expression passed in
            object value = Evaluate(stmt.expression);
            // Converts the value to string and prints it out to screen
            Console.WriteLine(Stringify(value));
            // return null to end function
            return null;
        }
        
        /*
        Vistor method for a return statement
        */
        public object VisitReturnStmt(Stmt.Return stmt){
            // create a new value to return
            object val = null;
            // If the return value is not null, evaluate statement and get value
            if(stmt.value != null) val= Evaluate(stmt.value);
            // return the value from the statement
            throw new Return(val);
        }

        /*
        Visitor method for variable statement
        */
        public object VisitVarStmt(Stmt.Var stmt){
            // Create a new value object
            object value = null;
            // If there is a variable initialized, evaluate the statement
            if (stmt.initializer != null){
                value = Evaluate(stmt.initializer);
            }
            // Define the variable in the environment
            environment.Define(stmt.name.lexeme, value);

            return null;
        }

        /*
        Use C#'s while to check for the condition's truthiness, then executes the body until it becomes falsey
        */
        public object VisitWhileStmt(Stmt.While stmt) {
            while (IsTruthy(Evaluate(stmt.condition))) {
                Execute(stmt.body);
            }

            return null;
        }

        /*
        Visitor method for Assign Expression
        */
        public object VisitAssignExpr(Expr.Assign expr){
            // Set value to be the value of the Exoressuib
            object value = Evaluate(expr.value);
            // Assign the variable value in the environment
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

        /* 
        Visitor method for Call Expresssion\

        Allows function to be called in Lox with arguments
        */
        public object VisitCallExpr(Expr.Call expr) {
            // Set the Calle to the expression callee
            object callee = Evaluate(expr.callee);

            // Create a new list to store the function calls arguments
            List<object> arguments = new List<object>();

            // Assign the arguements
            foreach (Expr argument in expr.arguments){
                arguments.Add(Evaluate(argument));
            }

            // Check if callee is a callable type and can be called
            if(!(callee is LoxCallable)) {
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            }

            // Create a new function and set it to Callee
            LoxCallable function = (LoxCallable) callee;
            // Check how many arguments were given and needed
            if(arguments.Count != function.Arity()){
                throw new RuntimeError(expr.paren, "Expected " + function.Arity() + " arguments but got " + arguments.Count + ".");
            }
            // Return the call on the arguments 
            return function.Call(this, arguments);
        }

        /*
        The Interpret method for Lox.
        Allows us to accept a list of statments and execute on them.
        */
        public void Interpret(List<Stmt> statements) {
            try {
                // For every statement execute the statement
                foreach (Stmt statement in statements){
                    Execute(statement);
                }
            }
            catch (RuntimeError error) {
                // If an error occured, throw an error so we know. 
                Lox.RuntimeError(error);
            }
        }


    }
}