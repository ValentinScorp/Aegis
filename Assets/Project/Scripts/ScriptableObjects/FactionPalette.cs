using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    [CreateAssetMenu(fileName = "FactionPalette", menuName = "Aegis/Faction Palette")]

    public class FactionPalette : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public FactionId id;
            public Color color;
        }

        [SerializeField] private Entry[] entries;
        [SerializeField] private Color fallback = Color.magenta; 
        public Color GetColor(FactionId id)
        {
            foreach (var e in entries)
                if (e.id.Equals(id)) 
                    return e.color;
                    
            Debug.LogWarning($"[{GetType().Name}] Unknown factionId: {id}", this);
            return fallback;
        }
    }
}