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
            _world.UnitCreated += OnEntityCreated;
        }
        private void Start()
        {
            Unit player = null;

            foreach (var p in FindObjectsByType<UnitSpawnPoint>(FindObjectsSortMode.None)) {
                var unit = _world.CreateUnit(p.transform.position, p.FactionId, p.UnitType, _unitConfigRegistry.GetConfig(p.UnitType), _unitCommonConfig);
                if (p.IsPlayerControlled) {
                    if (player != null) Debug.LogWarning("Кілька UnitSpawnPoint з IsPlayerControlled!", p);
                    player = unit;
                }
            }
            if (player != null) _world.AssignPlayerUnit(player);
            else Debug.LogWarning("Жоден UnitSpawnPoint не позначений IsPlayerControlled.");
        }
        private void OnDisable()
        {
            if (_world != null) {
                _world.UnitCreated -= OnEntityCreated;
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
            if (entity is not Unit unit) return;

            var view = Instantiate(_humanoidUnit, unit.Position, unit.Rotation, transform);
            view.Initialize(unit.FactionId);
            view.Bind(unit);
            _views.Add(view);
        }
    }
}
