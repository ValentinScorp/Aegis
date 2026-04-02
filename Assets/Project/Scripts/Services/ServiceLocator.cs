using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T instance) where T : class => _services[typeof(T)] = instance;
        public static void Unregister<T>() where T : class => _services.Remove(typeof(T));
        public static T Get<T>() where T : class
        {
            T service = _services.TryGetValue(typeof(T), out var s) ? (T)s : null;
            if (service == null) {
                Debug.LogWarning($"Unable to get service {typeof(T).Name} from ServiceLocator!");
            }
            return service;
        }
        public static void LogRegisteredServices()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Registered services:");

            foreach (var kv in _services) {
                var type = kv.Key;
                var instance = kv.Value;
                sb.AppendLine($"- {type.FullName} => {(instance != null ? instance.GetType().FullName : "null")}");
            }

            Debug.Log(sb.ToString());
        }
    }
}
