using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class WorldView : MonoBehaviour
    {
        [SerializeField] private EntityViewRegistry entityViewRegistry;

        private List<EntityView> _views = new();
        private World _world;
        private void Awake()
        {
            _world = World.Instance;
            _world.EntityCreated += OnEntityCreated;
        }
        private void Start()
        {
            _world.SpawnUnits(entityViewRegistry);
        }
        private void OnDisable()
        {
            if (_world != null) {
                _world.EntityCreated -= OnEntityCreated;
            }
        }
        private void OnDestroy()
        {
            foreach (var view in _views) {
                if (view != null) view.Unbind();
            }
            _views.Clear();
        }
        private void OnEntityCreated(WorldEntity entity)
        {
            SetConfig(entity);
            CreateEntityView(entity);
        }
        private void SetConfig(WorldEntity entity)
        {
            if (entity is Unit unit) {
                unit.SetConfig(entityViewRegistry.GetConfig(unit.EntityType));
            }
        }
        private void CreateEntityView(WorldEntity entity)
        {
            EntityView prefab = null;

            if (entity is Unit unit)
                prefab = entityViewRegistry.GetPrefab(unit.EntityType);

            if (prefab == null) return;

            var view = Instantiate(prefab, entity.Position, entity.Rotation, transform);
            if (entity is Unit u) {
                view.Initialize(u.FactionId);
            }
            view.Bind(entity);            
            _views.Add(view);
        }
    }
}
