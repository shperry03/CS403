using System.Collections.Generic;

namespace project2
{
    /*
    Class for callible functions in lox
    allows us to split callable and non-callable objes
    */
    public interface LoxCallable
    {
        // Call function that exists for every callable object
        public object Call(Interpreter interpreter, List<object> arguments);
        // function that returns the number of args in a function
        public int Arity();
    }
}