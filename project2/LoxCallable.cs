using System.Collections.Generic;

namespace project2
{
    public interface LoxCallable
    {
        public object Call(Interpreter interpreter, List<object> arguments);
        public int Arity();
    }
}