using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class World
    {
        private static readonly World _instance = new World();
        private static int _instanceAccessCount;

        public static World Instance {
            get {
                _instanceAccessCount++;
                // Debug.Log($"World.Instance accessed {_instanceAccessCount} time(s).");
                return _instance;
            }
        }

        public event Action<WorldEntity> EntityCreated;
        private readonly List<WorldEntity> _entities = new();
        public IReadOnlyList<WorldEntity> Entities => _entities;

        private World()
        {
            CreateEntity(Vector3.zero, isEnemy: false);
            CreateEntity(new Vector3(-2f, 0.03f, 11f), isEnemy: true);

        }
        public WorldEntity CreateEntity(Vector3 position, bool isEnemy)
        {
            var entity = new WorldEntity();
            entity.SetPosition(position);
            entity.isEnemy = isEnemy;
            _entities.Add(entity);
            EntityCreated?.Invoke(entity);
            return entity;
        }
    }
}
