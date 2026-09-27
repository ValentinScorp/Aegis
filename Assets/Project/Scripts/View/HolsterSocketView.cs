using System.Collections.Generic;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class HolsterSocketView : WeaponSocketView
    {
        [SerializeField] private HolsterSocketId _id;
        public HolsterSocketId Id => _id;
        protected override (Vector3, Quaternion) GetAttachOffset(WeaponAttachConfig config) => config.GetAttach(_id);
    }
}