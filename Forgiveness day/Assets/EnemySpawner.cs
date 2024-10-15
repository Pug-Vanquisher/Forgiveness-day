using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Префаб дрона
    public Transform[] spawnPoints; // Места спавна

    private void Start()
    {
        EventManager.Instance.Subscribe("NoEnemies", SpawnEnemy);
        CheckForEnemies();
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe("NoEnemies", SpawnEnemy);
    }

    private void Update()
    {
        CheckForEnemies();
    }

    private void CheckForEnemies()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            EventManager.Instance.TriggerEvent("NoEnemies");
        }
    }

    private void SpawnEnemy()
    {
        // Выбираем случайную позицию спавна
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Spawned a new enemy!");
    }
}
