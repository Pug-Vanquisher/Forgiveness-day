using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    public bool isHeadCollider = false; // Установите для головного коллайдера в инспекторе

    private void OnTriggerEnter(Collider other)
    {
        // Здесь можно добавить другие действия при попадании, если нужно
    }
}
