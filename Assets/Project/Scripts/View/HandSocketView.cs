using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class HandSocketView : WeaponSocketView
    {
        [SerializeField] private HandSocketId _id;
        public HandSocketId Id => _id;
        protected override (Vector3, Quaternion) GetAttachOffset(WeaponAttachConfig config) => config.GetAttach(_id);
    }
}