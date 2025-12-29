namespace Ronin.Core
{
    public interface ITransition
    {
        IPredicate Condition { get; }
        IState To { get; }
    }
}
