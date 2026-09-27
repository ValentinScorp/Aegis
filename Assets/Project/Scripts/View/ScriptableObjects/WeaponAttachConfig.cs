using System;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    [CreateAssetMenu(fileName = "WeaponAttachConfig", menuName = "Aegis/Weapon Attach Config")]
    public class WeaponAttachConfig : ScriptableObject
    {
        [Serializable]
        public struct HandEntry
        {
            public HandSocketId SocketId;
            public Vector3 Position;
            public Vector3 RotationEuler;
        }

        [Serializable]
        public struct HolsterEntry
        {
            public HolsterSocketId SocketId;
            public Vector3 Position;
            public Vector3 RotationEuler;
        }
        [SerializeField] private WeaponTypeId _weaponType;
        [SerializeField] private Vector3 _defaultPosition;
        [SerializeField] private Vector3 _defaultRotationEuler;

        [SerializeField] private HandEntry[] _handOffsets;
        [SerializeField] private HolsterEntry[] _holsterOffsets;

        public WeaponTypeId WeaponType => _weaponType;

        public (Vector3 pos, Quaternion rot) GetAttach(HandSocketId socketId)
        {
            foreach (var e in _handOffsets)
                if (e.SocketId == socketId)
                    return (e.Position, Quaternion.Euler(e.RotationEuler));
            return (_defaultPosition, Quaternion.Euler(_defaultRotationEuler));
        }

        public (Vector3 pos, Quaternion rot) GetAttach(HolsterSocketId socketId)
        {
            foreach (var e in _holsterOffsets)
                if (e.SocketId == socketId)
                    return (e.Position, Quaternion.Euler(e.RotationEuler));
            return (_defaultPosition, Quaternion.Euler(_defaultRotationEuler));
        }
    }
}