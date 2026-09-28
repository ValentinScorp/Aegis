using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;

namespace Aegis.View
{
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private float _horizontalSpeed = 20f;
        [SerializeField] private float _gravity = 20f;
        [SerializeField] private float _arrowRadius = 0.05f;
        [SerializeField] private LayerMask _hitZoneMask;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _drag = 0.003f;
        [SerializeField] private float _safeDistance = 1.0f; // дистанція, на яку стріла не перевіряє зони тіла того хто її випустив
        [SerializeField] private float _maxLifetime = 10f;
        private int _allHitMasks;
        private Unit _owner;
        private Vector3 _velocity;
        private float _age;
        private bool _hasHit;
        private float _travelled;
        private float _accumulator;
        private readonly RaycastHit[] _hits = new RaycastHit[8];
        private void Awake()
        {
            _allHitMasks = _hitZoneMask | _groundMask;
        }
        // public void Launch(Unit owner, WorldEntity target)
        // {
        //     _owner = owner;

        //     Vector3 aimPoint = target.Position + Vector3.up * 1f;
        //     Vector3 delta = aimPoint - transform.position;
        //     Vector3 flat = new Vector3(delta.x, 0f, delta.z);

        //     float t = Mathf.Max(flat.magnitude / _horizontalSpeed, 0.05f);

        //     // швидкість, з якою через t секунд стріла опиниться в aimPoint
        //     _velocity = new Vector3(
        //         delta.x / t,
        //         delta.y / t + 0.5f * _gravity * t,
        //         delta.z / t);

        //     transform.rotation = Quaternion.LookRotation(_velocity);
        // }

        public void Launch(Unit owner, Vector3 aimPoint, Vector3 targetVelocity)
        {
            _owner = owner;

            _velocity = ArrowBallistics.SolveLeadVelocity(
                transform.position, aimPoint, targetVelocity,
                _horizontalSpeed, _gravity, _drag);

            transform.rotation = Quaternion.LookRotation(_velocity);
        }

        private void Update()
        {
            if (_hasHit) return;

            _age += Time.deltaTime;
            _accumulator += Time.deltaTime;

            while (_accumulator >= ArrowBallistics.SimStep) {
                _accumulator -= ArrowBallistics.SimStep;
                if (!StepOnce()) return;   // влучили, стріла вже знищена
            }

            if (_age >= _maxLifetime)
                Destroy(gameObject);
        }

        private bool StepOnce()
        {
            Vector3 prev = transform.position;
            Vector3 next = prev;
            ArrowBallistics.Step(ref next, ref _velocity, ArrowBallistics.SimStep, _gravity, _drag);

            Vector3 delta = next - prev;
            float dist = delta.magnitude;

            if (dist > 0.0001f) {
                Vector3 dir = delta / dist;
                int count = Physics.SphereCastNonAlloc(prev, _arrowRadius, dir, _hits, dist,
                                                       _allHitMasks, QueryTriggerInteraction.Collide);

                int best = -1;
                float bestDist = float.MaxValue;
                for (int i = 0; i < count; i++) {
                    bool isZone = (_hitZoneMask.value & (1 << _hits[i].collider.gameObject.layer)) != 0;
                    if (isZone && _travelled < _safeDistance) continue;

                    if (_hits[i].distance < bestDist) {
                        bestDist = _hits[i].distance;
                        best = i;
                    }
                }

                if (best >= 0) {
                    Hit(_hits[best]);
                    return false;
                }
            }

            _travelled += dist;
            transform.position = next;
            transform.rotation = Quaternion.LookRotation(_velocity);
            return true;
        }
        private void Hit(RaycastHit hit)
        {
            _hasHit = true;
            transform.position = hit.point;

            // влучили в зону: шкода саме тій зоні й тому юніту, до якого вона належить
            if (hit.collider.TryGetComponent(out HitZoneView zone))
                _owner?.ApplyProjectileDamage(zone.Owner, zone.BodyPart);

            // інакше це земля або перешкода: промах
            Destroy(gameObject);
        }
    }
}