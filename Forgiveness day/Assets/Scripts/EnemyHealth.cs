using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public bool alreadyDied;
    public GameObject flame;
    private bool isBurning;
    private float burnDuration = 5f;
    private float burnDamageInterval = 1f;

    private void Start()
    {
        flame.SetActive(false);
        currentHealth = maxHealth;
        alreadyDied = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0 && !alreadyDied)
        {
            alreadyDied = true;
            Die();
        }
    }

    private void Die()
    {
        EventManager.Instance.TriggerEvent("EnemyKilled");
        Destroy(gameObject);
    }

    public void StartBurning()
    {
        if (isBurning)
        {
            StopCoroutine("BurnEffect");
        }
        StartCoroutine("BurnEffect");
    }

    private IEnumerator BurnEffect()
    {
        flame.SetActive(true);
        isBurning = true;
        float burnTime = burnDuration;

        while (burnTime > 0)
        {
            TakeDamage(5);
            yield return new WaitForSeconds(burnDamageInterval);
            burnTime -= burnDamageInterval;
        }

        isBurning = false;
        flame.SetActive(false);
    }
}
