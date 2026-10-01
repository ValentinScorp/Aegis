using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class UnitAimTwist : MonoBehaviour
    {
        [SerializeField] private Transform[] _spineBones;   // від нижньої до верхньої
        [SerializeField] private float _blendSpeed = 8f;
        [SerializeField] private float _maxAngle = 120f;

        private Unit _unit;
        private float _weight;

        public void Bind(Unit unit) => _unit = unit;
        public void Unbind() => _unit = null;

        private void LateUpdate()
        {
            float target = (_unit != null && _unit.IsAiming) ? 1f : 0f;
            _weight = Mathf.MoveTowards(_weight, target, _blendSpeed * Time.deltaTime);

            if (_unit == null || _weight <= 0.001f || _spineBones == null || _spineBones.Length == 0) return;

            Vector3 aim = _unit.AimDirection;
            aim.y = 0f;
            if (aim.sqrMagnitude < 0.0001f) return;

            float yaw = Mathf.Clamp(Vector3.SignedAngle(transform.forward, aim, Vector3.up), -_maxAngle, _maxAngle);
            float perBone = yaw * _weight / _spineBones.Length;

            foreach (var bone in _spineBones)
                bone.rotation = Quaternion.AngleAxis(perBone, Vector3.up) * bone.rotation;
        }
    }
}