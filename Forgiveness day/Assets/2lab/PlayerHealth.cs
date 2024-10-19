using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private void Start()
    {
        // Подписка
        currentHealth = maxHealth;
        EventManager.Instance.Subscribe("Heal", Heal);
    }

    private void OnDestroy()
    {
        // Отписка
        EventManager.Instance.Unsubscribe("Heal", Heal);
    }

    private void Heal()
    {
        currentHealth = Mathf.Min(currentHealth + 20f, maxHealth);
        Debug.Log("Текущее здоровье после хила: " + currentHealth);
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
        Debug.Log("Помер");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
