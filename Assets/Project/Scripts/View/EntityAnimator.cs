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
        private int _currentStateHash;
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int SwordAttackHash = Animator.StringToHash("SwordAttack");
        private static readonly int BowShootHash = Animator.StringToHash("BowShoot");
        private static readonly int WalkHash = Animator.StringToHash("Walk");

        private static readonly int WalkSpeedHash = Animator.StringToHash("WalkSpeed");
        private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");


        public bool IsWalking => _currentStateHash == WalkHash;
        public void SetWalkSpeed(float speed) => _animator.SetFloat(WalkSpeedHash, speed);

        private void Awake()
        {
            _animator = ComponentResolver.Require(this, GetComponentInChildren<Animator>());
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
        // private void Update()
        // {
        //     if (_currentStateHash != WalkHash || _unit?.Config == null || _movement == null) return;

        //     float multiplier = _unit.Config.WalkAnimationSpeedMultiplier;
        //     _animator.SetFloat(WalkSpeedHash, _movement.NormalizedSpeed * multiplier);
        // }
        public void PlayAttack(WeaponType weaponType, float animSpeed)
        {
            int attackHash = Animator.StringToHash("SwordAttack");
            switch (weaponType) {
                case WeaponType.OneHandSword:
                    attackHash = Animator.StringToHash("SwordAttack");
                    break;
                case WeaponType.Bow:
                    attackHash = Animator.StringToHash("BowShoot");
                    break;
                default:
                    Debug.LogWarning("[EntityAnimator] Undefined attack animation hash!");
                    break;

            }
            _animator.SetFloat(AttackSpeedHash, animSpeed);
            PlayOnce(attackHash);
        }
        public void PlayIdle()
        {
            PlayLooping(IdleHash);
        }
        public void PlayWalk(float speed)
        {
            SetWalkSpeed(speed);
            PlayLooping(WalkHash);
        }
        public void PlayDeath()
        {
            PlayOnce(DeathHash);
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