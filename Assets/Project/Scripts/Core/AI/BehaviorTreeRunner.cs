using UnityEngine;

namespace Aegis.Core.AI
{
    public class BehaviorTreeRunner
    {
        private readonly BTNode _root; private readonly BTContext _ctx;
        public BehaviorTreeRunner(Unit unit, BTNode root)
        {
            _root = root;
            _ctx = new BTContext {
                Unit = unit
            };
        }
        public void Tick(float dt)
        {
            _ctx.DeltaTime = dt;
            _root.Tick(_ctx);
        }
        public void Stop() => _root.Abort(_ctx);
    }
}