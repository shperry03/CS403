
using System.Collections.Generic;

namespace project2
{
    /*
    Environemtn class for storing and assigning variables in the interpreter's Environment.
    */
    public class Environment {

        // Environment variable that contatines the dictionary
        public Environment enclosing {get; private set; }
        // Dictionary that stores the variables and their assigned values
        private Dictionary<string, object> values = new Dictionary<string, object>();
        // Constructor 
        public Environment(){
            this.enclosing = null;
        }
        // Constructor with value
        public Environment(Environment enclosing){
            this.enclosing = enclosing;
        }

        // Gets and returns a variable value based on the name given
        public object Get(Token name){

            if (values.TryGetValue(name.lexeme, out var value)){
                // If a var exists in environment, return its value
                return value;
            }
            // Check if the environment exists
            if (enclosing != null) return enclosing.Get(name);

            // if it falls through, the variables don't exist
            throw new RuntimeError(name, "Undefined Variable '" + name.lexeme + "'.");
        }

        // Assign function for variables in an environment
        public void Assign(Token name, object value){
            // If the var exists, update its value
            if (values.ContainsKey(name.lexeme)){
                values[name.lexeme] = value;
                return;
            }
            // assign the value as long as the env isn't null
            if (enclosing != null){
                enclosing.Assign(name,value);
                return;
            }

            // The variable is undefined if falls through
            throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
        }

        // Define function for variables
        public void Define(string name, object value){
            // Create variable name and assign value
            values[name] = value;
        }






    }

}