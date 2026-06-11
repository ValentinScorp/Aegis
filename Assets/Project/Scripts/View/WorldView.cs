using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class WorldView : MonoBehaviour
    {
        [SerializeField] private EntityView _entityPrefab;

        private List<EntityView> _views = new();
        private World _world;
        private void Awake()
        {
            _world = World.Instance;
        }
        private void Start()
        {
            foreach (var entity in _world.Entities) {
                CreateEntityView(entity);
            }
            _world.EntityCreated += OnEntityCreated;
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
            var view = Instantiate(_entityPrefab, entity.Position, entity.Rotation, transform);
            if (entity is Unit unit) {
                view.Initialize(unit.FactionId);
            }
            view.Bind(entity);
            _views.Add(view);
        }
    }
}
