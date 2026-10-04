namespace Aegis.Core.AI
{
    public class Sequence : BTNode
    {
        private readonly BTNode[] _children; private int _running = -1;
        public Sequence(params BTNode[] children) => _children = children;

        public override BTStatus Tick(BTContext c)
        {
            for (int i = 0; i < _children.Length; i++) {
                var s = _children[i].Tick(c);
                if (s == BTStatus.Success) continue;

                if (_running != -1 && _running != i) 
                    _children[_running].Abort(c);

                _running = s == BTStatus.Running ? i : -1;
                return s;
            }
            _running = -1;
            return BTStatus.Success;
        }
        public override void Abort(BTContext c)
        {
            if (_running != -1) {
                _children[_running].Abort(c);
                _running = -1;
            }
        }
    }
}