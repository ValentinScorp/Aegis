using UnityEngine;
using System;

namespace Aegis.View
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        public event Action ReleaseArrow;
        public event Action MeleeHit;

        void ReleaseArrowEvent()
        {
            ReleaseArrow?.Invoke();
        }
        void MeleeHitEvent()
        {
            MeleeHit?.Invoke();
        }
    }
}