using UnityEngine;

/// <summary>
/// Prevent instantiating and using tons of copies of materials
/// </summary>
public class FlowerSharedMaterials : MonoBehaviour {

    private static FlowerSharedMaterials _instance;
    public static FlowerSharedMaterials Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private Material[] _materials;

    private void Awake()
    {
        _instance = this;
    }

    public Material GetSharedMaterial(FlowerComponent.State type)
    {
        return _materials[(int)type];
    }
}
