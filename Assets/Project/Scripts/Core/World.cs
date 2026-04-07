using System;
using System.Collections.Generic;
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
            CreateEntity(new Vector3(-2f, 0.03f, 11f), factionId: 1);
            CreateEntity(new Vector3(6f, 0.03f, 11f), factionId: 2);
        }
        public WorldEntity CreateEntity(Vector3 position, int factionId)
        {
            var entity = new Unit(factionId);
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
