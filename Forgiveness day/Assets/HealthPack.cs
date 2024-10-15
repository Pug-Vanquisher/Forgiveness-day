using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ����������� �������� ������
            EventManager.Instance.TriggerEvent("Heal");
            Destroy(gameObject); // ������� ������� ����� �������
        }
    }
}
