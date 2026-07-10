using UnityEngine;
using System;

namespace Aegis.View
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        public event Action ReleaseArrow;
        public event Action SwordHit;

        void ReleaseArrowEvent()
        {
            ReleaseArrow?.Invoke();
        }
        void SwordHitEvent()
        {
            SwordHit?.Invoke();
        }
    }
}