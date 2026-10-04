namespace Aegis.Core.AI
{
    public abstract class BTAction : BTNode
    {
        private bool _active;

        public sealed override BTStatus Tick(BTContext c)
        {
            if (!_active) {
                _active = true;
                c.Unit.DebugActivity = GetType().Name;
                OnEnter(c);
            }
            var s = OnTick(c);
            if (s != BTStatus.Running) {
                _active = false;
                OnExit(c);
            }
            return s;
        }
        public sealed override void Abort(BTContext c)
        {
            if (_active) {
                _active = false;
                OnExit(c);
            }
        }

        protected virtual void OnEnter(BTContext c) { }
        protected abstract BTStatus OnTick(BTContext c);
        protected virtual void OnExit(BTContext c) { }
    }
}