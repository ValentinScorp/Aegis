using System.Collections.Generic;
using UnityEngine;

namespace Aegis.View
{
    public class ProjectilePool : MonoBehaviour
    {
        private readonly Dictionary<ProjectileView, Stack<ProjectileView>> _free = new();

        public ProjectileView Spawn(ProjectileView prefab, Vector3 pos, Quaternion rot)
        {
            if (!_free.TryGetValue(prefab, out var stack))
                _free[prefab] = stack = new Stack<ProjectileView>();

            ProjectileView p = null;
            // пропускаємо знищені (стріла застрягла в юніті, якого видалили)
            while (stack.Count > 0 && p == null)
                p = stack.Pop();

            if (p == null) {
                p = Instantiate(prefab, transform);
                p.Init(this, prefab);
            }

            p.transform.SetParent(transform, worldPositionStays: false);
            p.transform.SetPositionAndRotation(pos, rot);
            p.gameObject.SetActive(true);
            p.OnSpawn();
            return p;
        }

        public void Release(ProjectileView p)
        {
            if (p == null) return;
            p.gameObject.SetActive(false);
            p.transform.SetParent(transform, worldPositionStays: false);
            _free[p.Prefab].Push(p);
        }
    }
}