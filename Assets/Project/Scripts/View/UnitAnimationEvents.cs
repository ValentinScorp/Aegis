using System;
using UnityEngine;

namespace Aegis.View
{
    public class UnitAnimationEvents : MonoBehaviour
    {
        public event Action AttackHit;

        private void OnAttackHit()
        {
            Debug.Log("Attack!!!!");
            AttackHit?.Invoke();
        }
    }
}