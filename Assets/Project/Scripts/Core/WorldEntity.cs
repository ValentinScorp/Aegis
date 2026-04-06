using System;
using UnityEngine;

namespace Aegis.Core
{
    public class WorldEntity
    {
        private Vector3 _position;
        public Vector3 Position {
            get => _position;
            set {
                if (_position != value) {
                    _position = value;
                    ChangedPosition?.Invoke(_position);
                }
            }
        }
        public Quaternion Rotation;

        public event Action <Vector3> ChangedPosition;
    }
}
