using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;


namespace Aegis.View
{
    public class HitZoneView : MonoBehaviour
    {
        [SerializeField] private BodyPartId _bodyPartId;
        private Collider _collider;
        public BodyPartId BodyPart => _bodyPartId;
        public WorldEntity Owner { get; private set; }
        public Vector3 Center => _collider != null ? _collider.bounds.center : transform.position;

        private void Awake()
        {
            _collider = ComponentResolver.ResolveOrFind(this, _collider);
            
        }
        public void Bind(WorldEntity owner)
        {
            Owner = owner;
        }

        internal void Unbind()
        {
            Owner = null;
        }
    }
}