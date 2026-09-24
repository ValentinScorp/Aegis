using System;
using System.Collections.Generic;
using Aegis.View;
using UnityEngine;

namespace Aegis.Core
{
    public class World : IWorldUpdatable
    {
        private static readonly World _instance = new World();

        public static World Instance {
            get => _instance;
        }

        private readonly List<WorldEntity> _entities = new();
        public Unit PlayerUnit { get; private set; }
        public event Action<WorldEntity> UnitCreated;
        public event Action<Unit> PlayerUnitAssigned;
        public IReadOnlyList<WorldEntity> Entities => _entities;

        private World()
        {
        }
        public Unit CreateUnit(Vector3 position, FactionId factionId, UnitType type, UnitConfig config, UnitCommonConfig common)
        {
            var unit = new Unit(position, factionId, type, config, common);
            _entities.Add(unit);
            UnitCreated?.Invoke(unit);
            return unit;
        }
        public void AssignPlayerUnit(Unit unit)
        {
            if (PlayerUnit is not null) {
                PlayerUnit.SetPlayerControlled(false);
            }
            PlayerUnit = unit;
            unit.SetPlayerControlled(true);
            PlayerUnitAssigned?.Invoke(unit);
        }
        public void OnInteractionsUpdate()
        {
            foreach (var entity in _entities) {
                if (entity is Unit unit) unit.UpdateInteractions(_entities);
            }
        }
        public void OnActionsUpdate(float deltaTime)
        {
            foreach (var entity in _entities) {
                if (entity is Unit unit) unit.UpdateActions(deltaTime);
            }
        }
    }
}
