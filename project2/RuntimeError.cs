namespace project2
{

    /*
    Lox's RuntimeError will implement C#'s SystemException. 
    */
    public class RuntimeError : System.SystemException {
        public Token token; // Track the token that identifies where error came from

        // Calls SystemException's constructor with the passed in message, which reports to the console.
        public RuntimeError(Token token, string message) : base(message) {
            // super(message);
            this.token = token;
        }

        public RuntimeError(){
        }
    }
}