using UnityEngine;

public class AttackStrategy : IDroneStrategy
{
    private float attackCooldown = 2f; // Задержка между выстрелами
    private float lastAttackTime = 0f; // Время последнего выстрела

    public void Execute(DroneAI drone)
    {
        // Смотрим на игрока и поддерживаем высоту
        drone.LookAtPlayer();
        drone.MaintainHeight();

        // Проверяем, может ли дрон стрелять
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            drone.Shoot(); // Выполняем стрельбу
            lastAttackTime = Time.time; // Обновляем время последнего выстрела
        }
    }
}
