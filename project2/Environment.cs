
using System.Collections.Generic;

namespace project2
{
    public class Environment {

        public Environment enclosing {get; private set; }
        private Dictionary<string, object> values = new Dictionary<string, object>();
        
        public Environment(){
            this.enclosing = null;
        }

        public Environment(Environment enclosing){
            this.enclosing = enclosing;
        }
        public object Get(Token name){

            if (values.TryGetValue(name.lexeme, out var value)){
                return value;
            }

            if (enclosing != null) return enclosing.Get(name);

            throw new RuntimeError(name, "Undefined Variable '" + name.lexeme + "'.");
        }

        public void Assign(Token name, object value){
            if (values.ContainsKey(name.lexeme)){
                values[name.lexeme] = value;
                return;
            }

            if (enclosing != null){
                enclosing.Assign(name,value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
        }
        public void Define(string name, object value){
            values[name] = value;
        }






    }

}