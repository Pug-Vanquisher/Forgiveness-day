using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public Mesh bulletHoleMesh; 
    public Material bulletHoleMaterial; 

    private AssetFlyweight flyweight; // ������ ����� ��� � ��������

    void Start()
    {
        // ������� ������������ ������
        flyweight = new AssetFlyweight(bulletHoleMesh, bulletHoleMaterial);
    }

    public void SpawnBulletHole(Vector3 position, Quaternion rotation)
    {
        GameObject bulletHoleObject = new GameObject("BulletHole");

        MeshFilter meshFilter = bulletHoleObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = flyweight.SharedMesh; // O���� ���

        MeshRenderer meshRenderer = bulletHoleObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = flyweight.SharedMaterial; // O���� ��������

        // ������������� ������� � ������ �������
        bulletHoleObject.transform.position = position;
        bulletHoleObject.transform.rotation = rotation;
        bulletHoleObject.transform.localScale = new Vector3(0.012f, 0f, 0.012f);
        bulletHoleObject.layer = LayerMask.NameToLayer("Default"); 

        // ������� ���� ���� � ���������, �� ������� ��������� ������
        Debug.Log("Mesh reference: " + meshFilter.sharedMesh.GetInstanceID());
        Debug.Log("Material reference: " + meshRenderer.sharedMaterial.GetInstanceID());
    }

}
