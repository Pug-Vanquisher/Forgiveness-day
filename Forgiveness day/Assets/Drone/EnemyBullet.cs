using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f;
    public float bulletLifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, bulletLifetime); // Уничтожаем пулю через некоторое время, если не было столкновения
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Проверяем, попали ли в игрока
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Наносим урон игроку
            }

            Destroy(gameObject); // Уничтожаем пулю
        }
    }
}
