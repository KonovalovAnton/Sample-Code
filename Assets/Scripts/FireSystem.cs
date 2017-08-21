using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fire logic executor
/// </summary>
public class FireSystem : MonoBehaviour {

    static private Dictionary<int, FlowerComponent> _flowersOnFire;

    /// <summary>
    /// Cannot remove immidiately in foreach loop
    /// </summary>
    static private LinkedList<int> _sheduleRemove;

    /// <summary>
    /// Add new plants on fire only after the whole simulation frame
    /// </summary>
    static private LinkedList<FlowerComponent> _sheduleAdd;

    static FireSystem()
    {
        _flowersOnFire = new Dictionary<int, FlowerComponent>();
        _sheduleAdd = new LinkedList<FlowerComponent>();
        _sheduleRemove = new LinkedList<int>();
    }

    static public void AddFireFlower(FlowerComponent fc)
    {
        if(!_flowersOnFire.ContainsKey(fc.GetInstanceID()))
        {
            _flowersOnFire.Add(fc.GetInstanceID(), fc);
        }
    }

    /// <summary>
    /// Add at the end of the Update method after simulation iteration
    /// </summary>
    /// <param name="fc"></param>
    static public void ScheduleAdd(FlowerComponent fc)
    {
        _sheduleAdd.AddLast(fc);
    }

    /// <summary>
    /// Remooving plants without neighbors and dead plants
    /// </summary>
    /// <param name="id"></param>
    static public void ScheduleRemove(int id)
    {
        _sheduleRemove.AddLast(id);
    }

    /// <summary>
    /// Removing as click utility or total reset (not from simulation freach loop)
    /// </summary>
    /// <param name="id"></param>
    static public void Remove(int id)
    {
        if (_flowersOnFire.ContainsKey(id))
        {
            _flowersOnFire.Remove(id);
        }
    }

    void Update()
    {
        //set on fire
        foreach (var item in _flowersOnFire)
        {
            item.Value.SetNeighborsOnFire();
        }

        //remove unnececary objects to reduce iterations count
        foreach (var item in _sheduleRemove)
        {
            _flowersOnFire.Remove(item);
        }
        _sheduleRemove.Clear();

        //add new plants which were set on fire on this frame 
        foreach (var item in _sheduleAdd)
        {
            item.CurrentState = FlowerComponent.State.OnFire;
            AddFireFlower(item);
        }
        _sheduleAdd.Clear();
    }
}
