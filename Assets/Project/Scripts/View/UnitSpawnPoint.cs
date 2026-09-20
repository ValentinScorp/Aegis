using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class UnitSpawnPoint : MonoBehaviour
    {
        [field: SerializeField] public bool IsPlayerControlled { get; private set; }
        [field: SerializeField] public int FactionId { get; private set; } = 1;
        [field: SerializeField] public UnitType UnitType { get; private set; } = UnitType.Archer;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}