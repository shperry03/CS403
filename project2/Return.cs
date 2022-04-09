namespace project2
{
    // Return class of lox objects
    public class Return : RuntimeError
    {
        // Value that we return
        public object val;
        // Return contructor that sets the value
        public Return(object value){
            this.val = value;
        }
    }
}