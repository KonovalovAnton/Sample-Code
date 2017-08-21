using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plant component with the logic to build neighbors graph and simulate fire
/// </summary>
public class FlowerComponent : MonoBehaviour {

    public enum State
    {
        Alive,
        OnFire,
        Dead
    }

    private const float FIRE_PROBA = 0.999f;
    private const float ANGLE_SPREAD_COEFFICIENT = 8f;

    [SerializeField]
    private Renderer _render;

    [SerializeField]
    private float _fireRadius;

    [SerializeField]
    private float _onFireTime;
    
    private float _t;

    private State _currentState;
    public State CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
            _render.sharedMaterial = FlowerSharedMaterials.Instance.GetSharedMaterial(_currentState);
        }
    }

    /// <summary>
    /// Collection to store nearest neighbors which could be fired up
    /// 
    /// Choosen dictionary because of the big number of delete/add calls
    /// </summary>
    Dictionary<int, FlowerComponent> _neighbors;

    /// <summary>
    /// Storing inactive neighbors that needs to be deleted to prevent dummy iterations
    /// 
    /// Do not use delegates here! delegate += uses memberwise clone of the parameter, 
    /// which generates a lot of garbage and drammaticaly droppes down performance (too many calls)
    /// </summary>
    LinkedList<int> _scheduleRemove;
    
    public bool PlayerCreated { get; set; } 

    private void Start () {
        CurrentState = State.Alive;
        InitNeighbors();
	}

    public void ToggleFire()
    {
        switch(CurrentState)
        {
            case State.Alive:
                CurrentState = State.OnFire;
                FireSystem.AddFireFlower(this);
                break;
            case State.OnFire:
            case State.Dead:
                ResetComponent();
                break;
        }
    }

    public void ResetComponent()
    {
        CurrentState = State.Alive;
        FireSystem.Remove(GetInstanceID());
        InitNeighbors();
        _t = 0;
    }

    /// <summary>
    /// Removed plants should delete themselves from their neighbors lists to prevent 
    /// access to destroyed objects (removing objects during simulation)
    /// </summary>
    public void RemoveFromNeighbors()
    {
        var currentKey = GetInstanceID();
        var count = _neighbors.Keys.Count;
        int[] keys = new int[count];
        _neighbors.Keys.CopyTo(keys, 0);
        for (int i = 0; i < count; i++)
        {
            _neighbors[keys[i]]._neighbors.Remove(currentKey);
        }
    }

    /// <summary>
    /// The more speed - the more is the chance to set neighbors on fire
    /// </summary>
    /// <returns>float [0,1]</returns>
    private float GetFireProbability()
    {
        return FIRE_PROBA - WindController.WindSpeed * (1 - FIRE_PROBA);
    }

    /// <summary>
    /// Neighbors sector size based on constant direction error and additional error from wind speed 
    /// </summary>
    /// <returns>float [25, 25 + 25/8]</returns>
    private float GetWindAngleSize()
    {
        return WindController.ANGLE_DELTA + WindController.WindSpeed / ANGLE_SPREAD_COEFFICIENT;
    }

    /// <summary>
    /// Finds angle between current plant and target
    /// </summary>
    /// <param name="target">Flowercompoents transform</param>
    /// <returns>float [0,360]</returns>
    private float GetAngleTo(Transform target)
    {
        var angle = Vector3.Angle(transform.forward, Vector3.Scale(transform.InverseTransformPoint(target.position), new Vector3(1, 0, 1)));
        angle = Vector3.Dot(Vector3.right, transform.InverseTransformPoint(target.position)) > 0.0 ? angle : 360 - angle;
        return Mathf.Abs(angle - WindController.WindAngle);
    }
    
    /// <summary>
    /// Probability based model of setting neighbors on fire
    /// </summary>
    private void TrySetOnFire()
    {
        if (Random.Range(0.0f, 1.0f) > GetFireProbability())
        {           
            FireSystem.ScheduleAdd(this);
        }
    }

    public void SetNeighborsOnFire()
    {
        Fire();
        FireTimer();

        if(_neighbors.Count == 0 || CurrentState != State.OnFire)
        {
            CurrentState = State.Dead;
            FireSystem.ScheduleRemove(GetInstanceID());
        }
    }
	
    /// <summary>
    /// Being on fire longer than @_onFiretime toggles to a dead(burned) state
    /// </summary>
    private void FireTimer()
    {
        _t += Time.deltaTime;
        if (_t > _onFireTime)
        {
            CurrentState = State.Dead;
            _neighbors.Clear();
        }
    }

    /// <summary>
    /// Try to set on fire all the neighbors in a segment, according to the wind direction
    /// </summary>
    private void Fire()
    {
        foreach (var item in _neighbors)
        {
            if (item.Value.CurrentState == State.Alive)
            { 
                var delta = GetAngleTo(item.Value.transform);
                var semiSegment = GetWindAngleSize();
                if (delta < semiSegment || delta > 360 - semiSegment)
                {
                    item.Value.TrySetOnFire();
                }
            }
            else
            {
                _scheduleRemove.AddLast(item.Key);
            }
        }

        foreach (var item in _scheduleRemove)
        {
            _neighbors.Remove(item);
        }

        _scheduleRemove.Clear();
    }

    private void AddNeighbor(FlowerComponent fc)
    {
        if(_neighbors == null)
        {
            _neighbors = new Dictionary<int, FlowerComponent>();
        }

        if(!_neighbors.ContainsKey(fc.GetInstanceID()))
        {
            _neighbors.Add(fc.GetInstanceID(), fc);
        }
    }

    /// <summary>
    /// Do sphere cast to find neighbors
    /// </summary>
    private void InitNeighbors()
    {
        _neighbors = new Dictionary<int, FlowerComponent>();
        _scheduleRemove = new LinkedList<int>();
        Collider[] cols = Physics.OverlapSphere(transform.position, _fireRadius);
        if(cols != null)
        {
            foreach(var col in cols)
            {
                FlowerComponent fc = col.GetComponent<FlowerComponent>();
                if(fc != null)
                {
                    AddNeighbor(fc);
                    fc.AddNeighbor(this);
                }
            }
        }
    }

    /// <summary>
    /// Debug fire toggle
    /// </summary>
    [ContextMenu("Toggle")]
    void Toggle()
    {
        ToggleFire();
    }


    /// <summary>
    /// Debug fire distance and neighbors
    /// </summary>
    /*private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _fireRadius);
    }*/

}
