namespace project2
{
    public class RuntimeError : System.SystemException {
        public Token token;

        public RuntimeError(Token token, string message) : base(message) {
            // super(message);
            this.token = token;
        }

        public RuntimeError(){
        }
    }
}