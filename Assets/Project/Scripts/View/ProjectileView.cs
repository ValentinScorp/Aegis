using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private float _horizontalSpeed = 20f;
        [SerializeField] private float _gravity = 20f;
        // [SerializeField] private float _speed = 20f;
        [SerializeField] private float _hitRadius = 0.5f;
        [SerializeField] private float _hitDistance = 0.1f;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _maxLifetime = 10f;
        private Unit _owner;
        private WorldEntity _target;
        private Vector3 _velocity;
        private float _age;
        private bool _hasHit;

        public void Launch(Unit owner, WorldEntity target)
        {
            _owner = owner;
            _target = target;
            Vector3 aimPoint = target.Position + Vector3.up * 1f;
            Vector3 delta = aimPoint - transform.position;
            Vector3 flat = new Vector3(delta.x, 0f, delta.z);

            float t = Mathf.Max(flat.magnitude / _horizontalSpeed, 0.05f);

            // швидкість, з якою через t секунд стріла опиниться в aimPoint
            _velocity = new Vector3(
                delta.x / t,
                delta.y / t + 0.5f * _gravity * t,
                delta.z / t);

            transform.rotation = Quaternion.LookRotation(_velocity);
        }

        private void Update()
        {
            if (_hasHit) return;

            float dt = Time.deltaTime;
            _age += dt;

            Vector3 prev = transform.position;
            _velocity.y -= _gravity * dt;
            Vector3 next = prev + _velocity * dt;

            // 1. Влучання в ціль: перевіряємо відрізок руху за кадр, а не лише точку,
            //    щоб швидка стріла не перестрибнула ціль між кадрами.
            if (_target != null) {
                Vector3 targetPos = _target.Position + Vector3.up * 1f;
                if (DistanceToSegment(targetPos, prev, next) <= _hitRadius) {
                    Hit();
                    return;
                }
            }
            // 2. Падіння на землю: промах, стріла зникає
            if (Physics.Linecast(prev, next, _groundMask)) {
                Destroy(gameObject);
                return;
            }

            transform.position = next;
            transform.rotation = Quaternion.LookRotation(_velocity);

            if (_age >= _maxLifetime)
                Destroy(gameObject);
        }
        private static float DistanceToSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            float len2 = ab.sqrMagnitude;
            if (len2 < 1e-6f) return Vector3.Distance(p, a);
            float k = Mathf.Clamp01(Vector3.Dot(p - a, ab) / len2);
            return Vector3.Distance(p, a + ab * k);
        }
        private void Hit()
        {
            _hasHit = true;
            _owner?.ApplyProjectileDamage(_target, BodyPartId.Torso);
            Destroy(gameObject);
        }
    }
}