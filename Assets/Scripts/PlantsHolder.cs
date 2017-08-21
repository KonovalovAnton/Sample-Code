using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generating and storing plants as objects
/// </summary>
public class PlantsHolder : MonoBehaviour
{
    /// <summary>
    /// Number of object to generate
    /// </summary>
    const int PLANTS_NUMBER = 12000;

    /// <summary>
    /// Probability of choosing eleent from collection
    /// </summary>
    const float RANDOM_PROBA = 0.005f;

    private static PlantsHolder _instance;
    public static PlantsHolder Instance
    {
        get
        {
            return _instance;
        }
    }

    //size configs
    #region Configs
    /// <summary>
    /// plane left lower conner
    /// </summary>
    private float _configMinX;

    /// <summary>
    /// plane right lower conner
    /// </summary>
    private float _configMaxX;

    /// <summary>
    /// plane left upper conner
    /// </summary>
    private float _configMinZ;

    /// <summary>
    /// plane right upper corner
    /// </summary>
    private float _configMaxZ;

    /// <summary>
    /// height from which to raycast down to terrain
    /// </summary>
    private float _configY;
    #endregion

    [SerializeField]
    TerrainCollider _terrainCollider;

    [SerializeField]
    Terrain _terrain;

    [SerializeField]
    GameObject _prefab;

    LinkedList<GameObject> _plants;
    LinkedList<GameObject> _playerCreatedPlants;

    void Start()
    {
        _instance = this;
        Init();
    }

    void Init()
    {
        _playerCreatedPlants = new LinkedList<GameObject>();
        Vector3 size = _terrain.terrainData.size;
        _configMinX = transform.position.x;
        _configMinZ = transform.position.z;
        _configMaxX = _configMinX + size.x;
        _configMaxZ = _configMinZ + size.z;
        _configY = transform.position.y + size.y;
    }

    /// <summary>
    ///  click-add utility
    /// </summary>
    /// <param name="point">raycasted target</param>
    public void AddAtPoint(Vector3 point)
    {
        GameObject go = GameObject.Instantiate(_prefab, FindPlace(point.x, point.z), Quaternion.identity, transform);
        _playerCreatedPlants.AddLast(go);
        go.GetComponent<FlowerComponent>().PlayerCreated = true;
    }

    /// <summary>
    /// click-remove utility
    /// </summary>
    /// <param name="f">plant to be removed</param>
    public void Remove(FlowerComponent f)
    {
        //generated plants are not destroyed and reused as object pool
        if (f.PlayerCreated)
        {
            _playerCreatedPlants.Remove(f.gameObject);
            DestroyImmediate(f.gameObject);
        }
        else
        {
            f.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// First generation uses instantiate, then move inactive objects from pool
    /// </summary>
    public void GeneratePlantsFunction()
    {
        Clear();
        if (_plants == null)
        {
            _plants = new LinkedList<GameObject>();
            Generate();
        }
        else
        {
            Move();
        }

    }

    /// <summary>
    /// Weird way to choose random plants and set on fire
    /// </summary>
    public void FireFewRandom()
    {
        if (_plants != null)
        {
            //choose from generation pool
            foreach (var item in _plants)
            {
                FlowerComponent f = item.GetComponent<FlowerComponent>();
                if (f.CurrentState == FlowerComponent.State.Alive && Random.Range(0.0f, 1.0f) < RANDOM_PROBA)
                {
                    f.ToggleFire();
                }
            }
        }

        //choose from user generated objects
        foreach (var item in _playerCreatedPlants)
        {
            FlowerComponent f = item.GetComponent<FlowerComponent>();
            if (f.CurrentState == FlowerComponent.State.Alive && Random.Range(0.0f, 1.0f) < 1.0f / _playerCreatedPlants.Count)
            {
                f.ToggleFire();
            }
        }
    }

    /// <summary>
    /// Move and activate plants from pool
    /// </summary>
    private void Move()
    {
        foreach (var item in _plants)
        {
            float x = Random.Range(_configMinX, _configMaxX);
            float y = Random.Range(_configMinZ, _configMaxZ);
            item.transform.position = FindPlace(x, y);
            item.GetComponent<FlowerComponent>().ResetComponent();
            item.SetActive(true);
        }
    }

    /// <summary>
    /// Empty pool generation
    /// </summary>
    void Generate()
    {
        for (int i = 0; i < PLANTS_NUMBER; i++)
        {
            float x = Random.Range(_configMinX, _configMaxX);
            float y = Random.Range(_configMinZ, _configMaxZ);
            GameObject go = GameObject.Instantiate(_prefab, FindPlace(x, y), Quaternion.identity, transform);
            _plants.AddLast(go);
        }
    }

    /// <summary>
    /// Clear pool setting it inactive, clear usergenerated models with Destroy
    /// </summary>
    public void Clear()
    {
        if (_plants != null)
        {
            foreach (var item in _plants)
            {
                item.SetActive(false);
            }
        }

        foreach (var item in _playerCreatedPlants)
        {
            DestroyImmediate(item);
        }
        _playerCreatedPlants.Clear();
    }

    /// <summary>
    /// Raycast down to terrain to find an equivalent 3d point
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    Vector3 FindPlace(float x, float z)
    {
        RaycastHit intersection = new RaycastHit();
        Ray originUp = new Ray(new Vector3(x, _configY, z), Vector3.down);
        _terrainCollider.Raycast(originUp, out intersection, _configY);
        return intersection.point;
    }

    /// <summary>
    /// Debug generate
    /// </summary>
    [ContextMenu("Generate")]
    void DebugGenerate()
    {
        GeneratePlantsFunction();
    }

    /// <summary>
    /// Debug generate
    /// </summary>
    [ContextMenu("ClearPlants")]
    void DeugClear()
    {
        Clear();
    }
}
