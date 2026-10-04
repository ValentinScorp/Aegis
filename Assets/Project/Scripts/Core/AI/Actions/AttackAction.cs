namespace Aegis.Core.AI
{
    public class AttackAction : BTAction
    {
        private float _timer; private bool _damageDone; private WorldEntity _target;

        protected override void OnEnter(BTContext c)
        {
            var u = c.Unit;
            _target = u.CurrentTarget;
            u.AttackTarget = _target;
            u.StopMovement();
            u.PerformAttackAction(_target);
            _timer = 0f; _damageDone = false;
        }
        protected override BTStatus OnTick(BTContext c)
        {
            var u = c.Unit;
            if (_target == null || u.CurrentTarget != _target) return BTStatus.Failure;

            _timer += c.DeltaTime;
            if (!_damageDone && _timer >= u.AttackTime * u.AttackEventTime) {
                u.PerformAttackImpact(_target, BodyPartId.Torso);
                _damageDone = true;
            }
            if (_timer >= u.AttackTime) {
                _timer = 0f; _damageDone = false;
                u.PerformAttackAction(_target);
            }
            return BTStatus.Running;
        }
    }
}