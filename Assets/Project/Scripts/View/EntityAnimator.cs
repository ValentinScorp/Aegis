using System;
using Aegis.Core;
using Aegis.Utilities;
using UnityEngine;

namespace Aegis.View
{
    public class EntityAnimator : MonoBehaviour
    {
        private Animator _animator;
        private Unit _unit;
        private EntityMovement _movement;
        private int _currentStateHash;
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int ShootHash = Animator.StringToHash("Shoot");
        private static readonly int WalkHash = Animator.StringToHash("Walk");

        private static readonly int WalkSpeedHash = Animator.StringToHash("WalkSpeed");
        private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");
        private static readonly int ShootSpeedHash = Animator.StringToHash("ShootSpeed");

        private void Awake()
        {
            _animator = ComponentResolver.Require(this, GetComponentInChildren<Animator>());            
            _movement = ComponentResolver.Require(this, GetComponent<EntityMovement>());
        }
        private void OnDestroy()
        {
        }
        public void Bind(Unit unit)
        {
            _unit = unit;
        }
        public void Unbind()
        {
            _unit = null;
        }
        private void Update()
        {
            if (_currentStateHash != WalkHash || _unit?.Config == null || _movement == null) return;

            float multiplier = _unit.Config.WalkAnimationSpeedMultiplier;
            _animator.SetFloat(WalkSpeedHash, _movement.NormalizedSpeed * multiplier);
        }
        public void PlayAttack(float animSpeed)
        {
            _animator.SetFloat(AttackSpeedHash, animSpeed);
            PlayOnce(AttackHash);
        }
        public void PlayShoot(float animSpeed)
        {
            _animator.SetFloat(ShootSpeedHash, animSpeed);
            PlayOnce(ShootHash);
        }
        public void PlayIdle()
        {
            PlayLooping(IdleHash);
        }
        public void PlayWalk(float speed)
        {
            _animator.SetFloat("WalkSpeed", speed);
            PlayLooping(WalkHash);
        }
        public void PlayDeath()
        {
            Debug.Log("Death animation!");
            // todo death animation
        }
        private void PlayOnce(int stateHash)
        {
            _currentStateHash = 0;
            _animator.Play(stateHash, 0, 0f);
            _currentStateHash = stateHash;
        }
        private void PlayLooping(int stateHash)
        {
            if (_currentStateHash == stateHash) return;
            _animator.Play(stateHash, 0, float.NegativeInfinity);
            _currentStateHash = stateHash;
        }
    }
}