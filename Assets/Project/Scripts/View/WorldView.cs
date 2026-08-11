using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class WorldView : MonoBehaviour
    {
        [SerializeField] private UnitConfigRegistry _unitConfigRegistry;
        [SerializeField] private UnitCommonConfig _unitCommonConfig;
        [SerializeField] private EntityView _humanoidUnit;

        private List<EntityView> _views = new();
        private World _world;
        private void Awake()
        {
            _world = World.Instance;
            _world.EntityCreated += OnEntityCreated;
        }
        private void Start()
        {
            _world.SpawnUnits(_unitConfigRegistry, _unitCommonConfig);
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
            CreateEntityView(entity);
        }

        private void CreateEntityView(WorldEntity entity)
        {
            EntityView prefab = null;

            if (entity is Unit unit)
                prefab = _humanoidUnit;

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
