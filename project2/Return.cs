namespace project2
{
    public class Return : RuntimeError
    {
        public object val;
        public Return(object value){
            this.val = value;
        }
    }
}