using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLifetime = 2f;
    private bool hasCollided = false;
    private AssetManager assetManager;

    private void Start()
    {
        assetManager = FindObjectOfType<AssetManager>();
        StartCoroutine(DestroyAfterLifetime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        if (collision.collider.CompareTag("Enemy"))
        {
            EnemyTakeDamage(collision.collider.gameObject);
        }
        else
        {
            CreateBulletHole(collision);
        }

        StartCoroutine(DestroyBulletAfterDelay(0.01f));
    }

    private void EnemyTakeDamage(GameObject enemy)
    {
        DroneAI enemyAI = enemy.GetComponent<DroneAI>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(5f);
        }
    }

    private void CreateBulletHole(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point + contact.normal * 0.01f;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        // Вызов создания легковестного объекта эффекта попадания
        assetManager.SpawnBulletHole(hitPoint, rotation);
    }

    private IEnumerator DestroyBulletAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(bulletLifetime);

        if (!hasCollided)
        {
            Destroy(gameObject);
        }
    }
}
