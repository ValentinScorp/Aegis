using UnityEngine;

namespace Aegis.Core
{
    public static class WorldBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _ = World.Instance;
        }
    }
}
