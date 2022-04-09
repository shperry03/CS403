using System.Collections.Generic;

namespace project2
{
    class LoxFunction : LoxCallable
    {
        public Stmt.Function declaration;
        
        public LoxFunction(Stmt.Function declaration){
            this.declaration = declaration;
        }

        public object Call(Interpreter interpreter, List<object> arguments){
            Environment environment = new Environment(project2.Interpreter.globals);
            for (int i = 0; i < declaration.param.Count; i++){
                environment.Define(declaration.param[i].lexeme, arguments[i]);
            }
             
            try{
                interpreter.ExecuteBlock(declaration.body, environment);
            }catch (Return returnValue){
                return returnValue.val;
            }

            return null;
        }

        public int Arity(){
            return declaration.param.Count;
        }

        public string toString(){
            return "<fn " + declaration.name.lexeme + ">";
        }
    }
}