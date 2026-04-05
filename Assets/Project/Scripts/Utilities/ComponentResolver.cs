using UnityEngine;

namespace Aegis.Utilities
{
    public static class ComponentResolver
    {
        public static T ResolveOrFind<T>(MonoBehaviour context, T component) where T : Component
        {
            if (component != null) return component;

            Debug.LogWarning($"[{context.GetType().Name}] {typeof(T).Name} not assigned, searching in hierarchy...", context);

            if (context.transform.parent != null) {
                component = context.transform.parent.GetComponentInChildren<T>();
            }

            if (component == null) {
                Debug.LogError($"[{context.GetType().Name}] {typeof(T).Name} not found!", context);
            } else {
                Debug.Log($"[{context.GetType().Name}] Found {typeof(T).Name}: {component.gameObject.name}", context);
            }

            return component;
        }
    }
}
