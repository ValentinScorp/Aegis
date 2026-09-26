using System.Collections.Generic;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class HolsterSocketView : WeaponSocketView
    {
        [SerializeField] private HolsterSocketId _id;
        public HolsterSocketId Id => _id;
        private static readonly Quaternion BowRotationOffset = Quaternion.Euler(90f, 0f, 0f);
    }
}