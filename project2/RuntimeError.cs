namespace project2
{
    class RuntimeError : System.SystemException {
        public readonly Token token;

        public RuntimeError(Token token, string message) : base(message) {
            // super(message);
            this.token = token;
        }
    }
}