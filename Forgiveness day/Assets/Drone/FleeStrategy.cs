using UnityEngine.AI;
using UnityEngine;

public class FleeStrategy : IDroneStrategy
{
    private float healRate = 1f;

    public void Execute(DroneAI drone)
    {
        if (!drone.Agent.hasPath)
        {
            Vector3 directionAwayFromPlayer = (drone.transform.position - drone.Player.position).normalized;
            Vector3 fleePosition = drone.transform.position + directionAwayFromPlayer * Random.Range(20f, 30f);
            fleePosition.y = 3f;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleePosition, out hit, 20f, 1))
            {
                drone.Agent.SetDestination(hit.position);
            }
        }

        drone.MaintainHeight();
        drone.StartHealing(healRate);
    }
}
