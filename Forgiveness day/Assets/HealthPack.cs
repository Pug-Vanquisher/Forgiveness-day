using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Увеличиваем здоровье игрока
            EventManager.Instance.TriggerEvent("Heal");
            Destroy(gameObject); // Удаляем аптечку после подбора
        }
    }
}
