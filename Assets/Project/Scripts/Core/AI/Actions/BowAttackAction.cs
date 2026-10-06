namespace Aegis.Core.AI
{
    public class BowAttackAction : BTAction
    {
        private const float FireAngle = 7f;
        private float _drawTimer, _cooldown;
        private WorldEntity _target;

        protected override void OnEnter(BTContext c)
        {
            var u = c.Unit;
            _target = u.CurrentTarget;
            u.AttackTarget = _target;
            u.FacingTarget = _target;
            u.StopMovement();
            _drawTimer = 0f;
            _cooldown = 0f;
            u.BeginDraw();
        }
        protected override void OnExit(BTContext c)
        {
            c.Unit.FacingTarget = null;
            c.Unit.CancelDraw();
        }
        protected override BTStatus OnTick(BTContext c)
        {
            var u = c.Unit;
            if (_target == null || u.CurrentTarget != _target) return BTStatus.Failure;

            if (_cooldown > 0f) {                       // грає Bow_Release
                _cooldown -= c.DeltaTime;

                if (_cooldown <= 0f) {
                    _drawTimer = 0f;
                    u.BeginDraw();
                }

                return BTStatus.Running;
            }
            _drawTimer += c.DeltaTime;                  // натяг іде під час повороту
            if (_drawTimer >= u.BowDrawSeconds && u.IsFacing(_target, FireAngle)) {
                u.ReleaseArrow(_target);
                _cooldown = UnityEngine.Mathf.Max(0.3f, u.AttackTime - u.BowDrawSeconds);
            }
            return BTStatus.Running;                    // натягнутий лук тримається, поки не наведеться
        }
    }
}