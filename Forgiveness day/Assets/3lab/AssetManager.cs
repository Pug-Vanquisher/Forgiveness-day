using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public Mesh bulletHoleMesh; 
    public Material bulletHoleMaterial; 

    private AssetFlyweight flyweight; // хранит общий меш и материал

    void Start()
    {
        // Создаем легковестный объект
        flyweight = new AssetFlyweight(bulletHoleMesh, bulletHoleMaterial);
    }

    public void SpawnBulletHole(Vector3 position, Quaternion rotation)
    {
        GameObject bulletHoleObject = new GameObject("BulletHole");

        MeshFilter meshFilter = bulletHoleObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = flyweight.SharedMesh; // Oбщий меш

        MeshRenderer meshRenderer = bulletHoleObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = flyweight.SharedMaterial; // Oбщий материал

        // Устанавливаем позицию и размер объекта
        bulletHoleObject.transform.position = position;
        bulletHoleObject.transform.rotation = rotation;
        bulletHoleObject.transform.localScale = new Vector3(0.012f, 0f, 0.012f);
        bulletHoleObject.layer = LayerMask.NameToLayer("Default"); 

        // Отладка айди меша и материала, на который ссылается объект
        Debug.Log("Mesh reference: " + meshFilter.sharedMesh.GetInstanceID());
        Debug.Log("Material reference: " + meshRenderer.sharedMaterial.GetInstanceID());
    }

}
