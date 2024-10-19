using UnityEngine;

public class SimplePrefabSpawner : MonoBehaviour
{
    public GameObject prefab; // Префаб, который будет спавниться (назначается в инспекторе)

    void Update()
    {
       // for (int i = 0; i < 100; i++) // Создаем 100 объектов
        //{
          //  SpawnPrefab();
        //}
    }

    void SpawnPrefab()
    {
        // Генерируем случайную позицию в пределах 20x20 по X и Z
        Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

        // Спавним префаб на этой позиции
        Instantiate(prefab, randomPosition, Quaternion.identity);
    }
}
