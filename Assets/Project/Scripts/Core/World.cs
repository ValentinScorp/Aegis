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

        public event Action<WorldEntity> EntityCreated;
        private readonly List<WorldEntity> _entities = new();
        public IReadOnlyList<WorldEntity> Entities => _entities;

        private World()
        {
        }
        public void SpawnUnits(UnitConfigRegistry unitConfigs, UnitCommonConfig unitCommonConfig)
        {
            CreateEntity(new Vector3(-4f, 0.03f, 11f), factionId: 1, UnitType.Archer, unitCommonConfig, unitConfigs.GetConfig(UnitType.Archer));
            //CreateEntity(new Vector3(-4f, 0.03f, 10f), factionId: 1, UnitType.Knight, unitCommonConfig, unitConfigs.GetConfig(UnitType.Knight));
            // CreateEntity(new Vector3(0f, 0.03f, 11f), factionId: 2, UnitType.Knight, unitCommonConfig, unitConfigs.GetConfig(UnitType.Knight));
            // CreateEntity(new Vector3(5f, 0.03f, 10f), factionId: 2, UnitType.Archer, unitCommonConfig, unitConfigs.GetConfig(UnitType.Archer));
            CreateEntity(new Vector3(4f, 0.03f, 10f), factionId: 3, UnitType.Archer, unitCommonConfig, unitConfigs.GetConfig(UnitType.Archer));
            // CreateEntity(new Vector3(4f, 0.03f, 10f), factionId: 3, UnitType.Knight, unitCommonConfig, unitConfigs.GetConfig(UnitType.Knight));
            // CreateEntity(new Vector3(8f, 0.03f, 11f), factionId: 4, UnitType.Archer, unitCommonConfig, unitConfigs.GetConfig(UnitType.Archer));
            // CreateEntity(new Vector3(8f, 0.03f, 10f), factionId: 4, UnitType.Archer, unitCommonConfig, unitConfigs.GetConfig(UnitType.Archer));
        }
        public WorldEntity CreateEntity(Vector3 position, int factionId, UnitType type, UnitCommonConfig unitCommonConfig, UnitConfig config)
        {
            var entity = new Unit(position, factionId, type, config, unitCommonConfig);
            entity.Position = position;
            _entities.Add(entity);
            EntityCreated?.Invoke(entity);
            return entity;
        }
        public void RemoveEntity(WorldEntity entity)
        {
            _entities.Remove(entity);
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
