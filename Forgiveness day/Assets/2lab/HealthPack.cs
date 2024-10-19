using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Активируем событие Heal
            EventManager.Instance.TriggerEvent("Heal");
            Destroy(gameObject);
        }
    }
}
