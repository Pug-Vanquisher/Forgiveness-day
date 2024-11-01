using System.Collections;
using UnityEngine;

public class ExplosiveBullet : Bullet
{
    public GameObject stickyBombPrefab;

    protected override void HandleDamage(Collider collider)
    {
        base.HandleDamage(collider);

        if (collider.CompareTag("Enemy"))
        {
            AttachStickyBomb(collider);
        }
    }

    private void AttachStickyBomb(Collider collider)
    {
        EnemyHealth enemyHealth = collider.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            Vector3 hitPoint = collider.ClosestPointOnBounds(transform.position);
            GameObject stickyBomb = Instantiate(stickyBombPrefab, hitPoint, Quaternion.identity, collider.transform);
        }
    }
}
