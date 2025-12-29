namespace Ronin.Core
{
    public class Transition : ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate pred)
        {
            To = to;
            Condition = pred;
        }
    }
}