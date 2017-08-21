using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickModeRemove : ClickMode
{
    public override void DoJob(RaycastHit hit)
    {
        FlowerComponent f = hit.collider.gameObject.GetComponent<FlowerComponent>();
        if (f != null)
        {
            PlantsHolder.Instance.Remove(f);
            f.RemoveFromNeighbors();
            FireSystem.Remove(f.GetInstanceID());
        }
    }
}
