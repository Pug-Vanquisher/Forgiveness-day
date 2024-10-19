using UnityEngine;

public class SimplePrefabSpawner : MonoBehaviour
{
    public GameObject prefab; // ������, ������� ����� ���������� (����������� � ����������)

    void Update()
    {
       // for (int i = 0; i < 100; i++) // ������� 100 ��������
        //{
          //  SpawnPrefab();
        //}
    }

    void SpawnPrefab()
    {
        // ���������� ��������� ������� � �������� 20x20 �� X � Z
        Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

        // ������� ������ �� ���� �������
        Instantiate(prefab, randomPosition, Quaternion.identity);
    }
}
