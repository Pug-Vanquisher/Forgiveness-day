using System.Collections;
using UnityEngine;

public class StickyBomb : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        EventManager.Instance.Subscribe("Kaboom", OnKaboom);
    }

    private void OnKaboom()
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(50);
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe("Kaboom", OnKaboom);
    }
}
