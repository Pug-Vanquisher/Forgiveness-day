using UnityEngine;

public class BulletHoleDestroyer : MonoBehaviour
{
    private float lifetime;

    public void SetLifetime(float lifetime)
    {
        this.lifetime = lifetime;
        Destroy(gameObject, lifetime);
    }
}
