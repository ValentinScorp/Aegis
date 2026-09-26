using System.Collections.Generic;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class HandSocketView : WeaponSocketView
    {
        [SerializeField] private HandSocketId _id;
        public HandSocketId Id => _id;
        private static readonly Quaternion BowRotationOffset = Quaternion.Euler(90f, 0f, 0f);
    }
}