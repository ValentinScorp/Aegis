using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _hitDistance = 0.1f;

        private Unit _owner;
        private WorldEntity _target;
        private bool _hasHit;

        public void Launch(Unit owner, WorldEntity target)
        {
            _owner = owner;
            _target = target;
        }

        private void Update()
        {
            if (_hasHit || _target == null) {
                if (_target == null) Destroy(gameObject);
                return;
            }

            Vector3 targetPos = _target.Position + Vector3.up * 1f;
            Vector3 toTarget = targetPos - transform.position;
            float distance = toTarget.magnitude;

            if (distance <= _hitDistance) {
                Hit();
                return;
            }

            Vector3 direction = toTarget / distance;
            transform.position += direction * _speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void Hit()
        {
            _hasHit = true;
            _owner?.PerformAttackDamage(_target);
            Destroy(gameObject);
        }
    }
}