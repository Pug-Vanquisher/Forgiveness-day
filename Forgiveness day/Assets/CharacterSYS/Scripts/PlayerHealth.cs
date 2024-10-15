using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        EventManager.Instance.Subscribe("Heal", Heal);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe("Heal", Heal);
    }

    private void Heal()
    {
        currentHealth = Mathf.Min(currentHealth + 20f, maxHealth); // Восстанавливаем 20 здоровья
        Debug.Log("Player healed! Current health: " + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead! Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезапуск сцены
    }
}
