using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;
using System;

namespace Aegis.View
{
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private float _horizontalSpeed = 20f;
        [SerializeField] private float _gravity = 20f;
        [SerializeField] private float _arrowRadius = 0.05f;
        [SerializeField] private LayerMask _hitZoneMask;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _drag = 0.003f;
        [SerializeField] private float _safeDistance = 1.0f; // дистанція, на яку стріла не перевіряє зони тіла того хто її випустив
        [SerializeField] private float _maxLifetime = 10f;
        [SerializeField] private float _stickDuration = 20f;
        [SerializeField] private float _playerShotSpeed = 35f;
        [SerializeField] private float _stickDepth = 0.15f;
        [SerializeField] private float _deathImpulse = 100f;
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
            _allHitMasks = _hitZoneMask | _groundMask | _obstacleMask;
        }

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

            Vector3 dir = _velocity.normalized;

            transform.rotation = Quaternion.LookRotation(dir);
            transform.position = hit.point + dir * _stickDepth;

            if (TryGetComponent(out Collider selfCollider))
                selfCollider.enabled = false;

            if (hit.collider.TryGetComponent(out HitZoneView zone)) {
                _owner?.ApplyProjectileDamage(zone.Owner, zone.BodyPart);

                var victimDbg = zone.Owner as Unit;
                var rbDbg = hit.collider.attachedRigidbody;
                // Debug.Log($"[Impulse] victim={(victimDbg != null)} alive={victimDbg?.IsAlive} " +
                //           $"rb={(rbDbg != null ? rbDbg.name : "null")} kinematic={(rbDbg != null && rbDbg.isKinematic)} " +
                //           $"impulse={_deathImpulse}");

                if (zone.Owner is Unit victim && !victim.IsAlive) {
                    var rb = hit.collider.attachedRigidbody;
                    if (rb != null && !rb.isKinematic)
                        rb.AddForceAtPosition(dir * _deathImpulse, hit.point, ForceMode.Impulse);
                }

                transform.SetParent(hit.collider.transform, worldPositionStays: true);
            }

            enabled = false;
            Destroy(gameObject, _stickDuration);
        }
        public void LaunchStraight(Unit owner, Vector3 aimPoint)
        {
            _owner = owner;

            Vector3 dir = (aimPoint - transform.position).normalized;
            _velocity = dir * _playerShotSpeed;

            transform.rotation = Quaternion.LookRotation(_velocity);
        }

        internal void LaunchDirection(Unit owner, Vector3 direction)
        {
            _owner = owner;
            _velocity = direction.normalized * _playerShotSpeed;
            transform.rotation = Quaternion.LookRotation(_velocity);
            // Debug.Log($"[Arrow] launch v={_velocity} pos={transform.position}");
        }
        internal void LaunchAtCrosshair(Unit owner, Ray cameraRay, float zeroDistance)
        {
            _owner = owner;
            Vector3 muzzle = transform.position;

            // точка на осі камери, на zeroDistance попереду дула (по глибині)
            float along = Mathf.Max(0f, Vector3.Dot(muzzle - cameraRay.origin, cameraRay.direction));
            Vector3 target = cameraRay.origin + cameraRay.direction * (along + zeroDistance);

            // компенсація просідання: стріла за час польоту t падає на 0.5·g·t²
            float t = (target - muzzle).magnitude / _playerShotSpeed;
            target += Vector3.up * (0.5f * _gravity * t * t);

            _velocity = (target - muzzle).normalized * _playerShotSpeed;
            transform.rotation = Quaternion.LookRotation(_velocity);
        }
    }
}