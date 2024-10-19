using UnityEngine;

public class AssetFlyweight
{
    // Класс хранения данных о меше и материале
    public Mesh SharedMesh { get; private set; }
    public Material SharedMaterial { get; private set; }

    public AssetFlyweight(Mesh sharedMesh, Material sharedMaterial)
    {
        SharedMesh = sharedMesh;
        SharedMaterial = sharedMaterial;
    }
}
