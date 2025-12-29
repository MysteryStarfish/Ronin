namespace Ronin.Core
{
    public class FuncPredicate : IPredicate
    {
        private readonly System.Func<bool> _func;
        public FuncPredicate(System.Func<bool> func)
        {
            _func = func;
        }
        public bool Evaluate()
        {
            return _func.Invoke();
        }
    }
}