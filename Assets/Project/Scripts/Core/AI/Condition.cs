namespace Aegis.Core.AI
{
    public class Condition : BTNode
    {
        private readonly System.Func<BTContext, bool> _f;
        public Condition(System.Func<BTContext, bool> f) => _f = f;
        public override BTStatus Tick(BTContext c) => _f(c) ? BTStatus.Success : BTStatus.Failure;
    }
}