using UnityEngine;

public class IncendiaryBullet : Bullet
{
    protected override void HandleDamage(Collider collider)
    {
        base.HandleDamage(collider);

        EnemyHealth enemyHealth = collider.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.StartBurning();
        }
    }
}
