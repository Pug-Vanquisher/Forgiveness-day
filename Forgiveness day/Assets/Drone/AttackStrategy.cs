using UnityEngine;

public class AttackStrategy : IDroneStrategy
{
    private float attackCooldown = 2f;
    private float lastAttackTime = 0f; 

    public void Execute(DroneAI drone)
    {
        drone.LookAtPlayer();
        drone.MaintainHeight();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            drone.Shoot(); // Выстрел
            lastAttackTime = Time.time;
        }
    }
}
