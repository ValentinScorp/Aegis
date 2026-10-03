using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;
using Aegis.Services;
using System;

namespace Aegis.View
{
    public class WorldView : MonoBehaviour
    {
        [SerializeField] private UnitConfigCatalog _unitConfigCatalog;
        [SerializeField] private UnitCommonConfig _unitCommonConfig;
        [SerializeField] private UnitView _humanoidUnit;
        [SerializeField] private PlayerInputListener _playerInputListener;

        private List<UnitView> _views = new();
        private World _world;
        private void Awake()
        {
            if (_unitConfigCatalog == null) Debug.LogWarning("[WorldView] No UnitConfigCatalog assigned!");
            if (_unitCommonConfig == null) Debug.LogWarning("[WorldView] No UnitCommonConfig assigned!");
            if (_humanoidUnit == null) Debug.LogWarning("[WorldView] No unit prefab assigned!");
            if (_playerInputListener == null) Debug.LogWarning("[WorldView] No PlayerInputListener assigned!");

            _world = World.Instance;
        }
        private void OnEnable()
        {
            if (_world is not null) {
                _world.UnitCreated += OnEntityCreated;
            }
            if(_playerInputListener != null) {
                _playerInputListener.ShowInfoHoldChanged += OnShowInfoHoldChanged;
            }
        }

        private void Start()
        {
            if (_world is null) return;

            Unit player = null;

            foreach (var spawnPoint in FindObjectsByType<UnitSpawnPoint>(FindObjectsSortMode.None)) {
                var unit = _world.CreateUnit(spawnPoint.transform.position, spawnPoint.transform.rotation,
                                                spawnPoint.FactionId, spawnPoint.UnitType,
                                                _unitConfigCatalog.GetConfig(spawnPoint.UnitType),
                                                _unitCommonConfig);
                if (spawnPoint.IsPlayerControlled) {
                    if (player != null)
                        Debug.LogWarning("Кілька UnitSpawnPoint з IsPlayerControlled!", spawnPoint);
                    player = unit;
                }
            }
            if (player != null)
                _world.AssignPlayerUnit(player);
            else
                Debug.LogWarning("Жоден UnitSpawnPoint не позначений IsPlayerControlled.");
        }
        private void OnDisable()
        {
            if (_world is not null) {
                _world.UnitCreated -= OnEntityCreated;
            }
            if(_playerInputListener != null) {
                _playerInputListener.ShowInfoHoldChanged -= OnShowInfoHoldChanged;
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
        private void OnShowInfoHoldChanged(bool hold)
        {
            foreach (var v in _views) {
                v.SetInfoVisible(hold);
            }
        }
    }
}
