using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletHolePrefab;
    public TrailRenderer trailRenderer;
    public float bulletLifetime = 2f;
    protected bool hasCollided = false;
    public int baseDamage = 20;

    protected void Start()
    {
        StartCoroutine(DestroyAfterLifetime());
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleDamage(collision.collider);
        }
        else
        {
            CreateBulletHole(collision);
        }

        StartCoroutine(DestroyBulletAfterDelay(0.01f));
    }

    protected virtual void HandleDamage(Collider collider)
    {
        EnemyHealth enemyHealth = collider.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            int damage = baseDamage;

            EnemyCollider enemyCollider = collider.GetComponent<EnemyCollider>();
            if (enemyCollider != null && enemyCollider.isHeadCollider)
            {
                damage *= 2; // ”рон в голову в 2 раза выше
            }

            enemyHealth.TakeDamage(damage);
        }
    }

    protected void CreateBulletHole(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point + contact.normal * 0.01f;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        Instantiate(bulletHolePrefab, hitPoint, rotation);
    }

    protected IEnumerator DestroyBulletAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    protected IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(bulletLifetime);

        if (!hasCollided)
        {
            Destroy(gameObject);
        }
    }
}
