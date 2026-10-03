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
        [SerializeField] private float _gizmoRadius = 0.2f;

        private void OnDrawGizmos()
        {
            Gizmos.color = palette != null ? palette.GetColor(FactionId) : Color.magenta;
            if (IsPlayerControlled)
                Gizmos.DrawSphere(transform.position + Vector3.up * 0.1f, _gizmoRadius);
            else
                Gizmos.DrawWireSphere(transform.position, _gizmoRadius);

            // Напрямок погляду юніта
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            Vector3 tip = origin + transform.forward * 1f;
            Gizmos.DrawLine(origin, tip);
            Gizmos.DrawLine(tip, tip - (transform.forward + transform.right) * 0.25f);
            Gizmos.DrawLine(tip, tip - (transform.forward - transform.right) * 0.25f);
        }
    }
}