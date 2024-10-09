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

        CreateBulletHole(collision);

        StartCoroutine(DestroyBulletAfterDelay(0.01f)); 
    }

    private void CreateBulletHole(Collision collision)
    {
        ContactPoint contact = collision.contacts[0]; 

        Vector3 hitPoint = contact.point + contact.normal * 0.01f; 
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal); // Правильная ориентация (ноу хомо)

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
