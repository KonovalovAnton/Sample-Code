using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickModeToggle : ClickMode
{
    public override void DoJob(RaycastHit hit)
    {
        FlowerComponent f = hit.collider.gameObject.GetComponent<FlowerComponent>();
        if (f != null)
        {
            f.ToggleFire();
        }
    }
}
