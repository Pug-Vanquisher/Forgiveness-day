using UnityEngine.AI;
using UnityEngine;

public class PatrolStrategy : IDroneStrategy
{
    public void Execute(DroneAI drone)
    {
        if (!drone.Agent.hasPath)
        {
            // Генерация случайной точки для патрулирования
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += drone.transform.position;
            randomDirection.y = 0;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, 1))
            {
                Vector3 targetPosition = hit.position;
                targetPosition.y = 3f; // Устанавливаем высоту
                drone.Agent.SetDestination(targetPosition);
            }
        }

        drone.MaintainHeight();
    }
}