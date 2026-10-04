namespace Aegis.Core.AI
{
    public abstract class BTNode
    {
        public abstract BTStatus Tick(BTContext c);
        public virtual void Abort(BTContext c) { }
    }
}