using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletHolePrefab;
    public TrailRenderer trailRenderer;
    public float bulletLifetime = 2f;
    private bool hasCollided = false;

    private void Start()
    {
        StartCoroutine(DestroyAfterLifetime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        if (collision.collider.CompareTag("Enemy"))
        {
            // Попадание во врага
            EnemyTakeDamage(collision.collider.gameObject);
        }
        else
        {
            // Создаем след от пули, если объект не является врагом
            CreateBulletHole(collision);
        }

        StartCoroutine(DestroyBulletAfterDelay(0.01f));
    }

    private void EnemyTakeDamage(GameObject enemy)
    {
        // Находим компонент врага и вызываем метод для нанесения урона
        DroneAI enemyAI = enemy.GetComponent<DroneAI>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(5f); // Наносим 10 единиц урона (или любое другое значение)
        }
    }

    private void CreateBulletHole(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        Vector3 hitPoint = contact.point + contact.normal * 0.01f;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        Instantiate(bulletHolePrefab, hitPoint, rotation);
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