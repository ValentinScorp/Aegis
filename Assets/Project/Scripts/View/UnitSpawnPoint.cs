using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class UnitSpawnPoint : MonoBehaviour
    {
        [field: SerializeField] public bool IsPlayerControlled { get; private set; }
        [field: SerializeField] public FactionId FactionId { get; private set; } = FactionId.Red;
        [field: SerializeField] public UnitType UnitType { get; private set; } = UnitType.Archer;
        [SerializeField] private FactionPalette palette;
        [SerializeField] private float _gizmoRadius = 0.5f;

        private void OnDrawGizmos()
        {
            Gizmos.color = palette != null ? palette.GetColor(FactionId) : Color.magenta;;
            Gizmos.DrawWireSphere(transform.position, _gizmoRadius);
        }
    }
}