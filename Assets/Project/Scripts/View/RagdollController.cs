using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Rigidbody[] _bodies;

    private void Awake()
    {
        _bodies = GetComponentsInChildren<Rigidbody>(true);
        SetRagdoll(false);
    }

    public void SetRagdoll(bool on)
    {
        foreach (var rb in _bodies)
            rb.isKinematic = !on;

        if (animator != null)
            animator.enabled = !on;
    }

    public void Die()
    {
        SetRagdoll(true);
    }
}