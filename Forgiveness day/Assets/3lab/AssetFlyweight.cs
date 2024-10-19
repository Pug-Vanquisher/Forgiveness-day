using UnityEngine;

public class AssetFlyweight
{
    // ����� �������� ������ � ���� � ���������
    public Mesh SharedMesh { get; private set; }
    public Material SharedMaterial { get; private set; }

    public AssetFlyweight(Mesh sharedMesh, Material sharedMaterial)
    {
        SharedMesh = sharedMesh;
        SharedMaterial = sharedMaterial;
    }
}
