using System.Collections.Generic;

namespace project2
{
    /*
    LoxFunction class for functions that are written 
    */
    class LoxFunction : LoxCallable
    {
        // Declaration of the funciton
        public Stmt.Function declaration;
        
        //COnstructor
        public LoxFunction(Stmt.Function declaration){
            this.declaration = declaration;
        }

        // The function that lets us call the declared function
        public object Call(Interpreter interpreter, List<object> arguments){
            // create new environment for the scope fo the function
            Environment environment = new Environment(project2.Interpreter.globals);
            // define all the parameters
            for (int i = 0; i < declaration.param.Count; i++){
                environment.Define(declaration.param[i].lexeme, arguments[i]);
            }
             
            try{
                // execute the function body in the environment
                interpreter.ExecuteBlock(declaration.body, environment);
            }catch (Return returnValue){
                return returnValue.val;
            }

            return null;
        }

        /*
        Returns the number of parameters for the function
        */
        public int Arity(){
            return declaration.param.Count;
        }

        // Returns the name of the function that has been declared
        public string toString(){
            return "<fn " + declaration.name.lexeme + ">";
        }
    }
}