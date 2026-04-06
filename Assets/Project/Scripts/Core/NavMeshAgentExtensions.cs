using UnityEngine.AI;

public static class NavMeshAgentExtensions
{
    public static bool ReachedDestinationOrGaveUp(this NavMeshAgent agent)
    {
        if (agent.pathPending) return false;

        if (agent.remainingDistance <= agent.stoppingDistance) {
            if (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.1f) {
                return true;
            }
        }

        return false;
    }
}