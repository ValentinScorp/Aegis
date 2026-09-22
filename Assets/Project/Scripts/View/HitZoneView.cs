using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class HitZoneView : MonoBehaviour
    {
        [SerializeField] private BodyPartId _id;
        public BodyPartId Id => _id;
        public WorldEntity Entity { get; private set; }

        public void Bind(WorldEntity entity) => Entity = entity;
    }
}