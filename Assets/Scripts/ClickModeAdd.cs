using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickModeAdd : ClickMode
{
    public override void DoJob(RaycastHit hit)
    {
        if(hit.point != null)
        {
            PlantsHolder.Instance.AddAtPoint(hit.point);
        }
    }
}

public abstract class ClickMode : MonoBehaviour
{
    public void Update()
    {
        Click();
    }

    public void Click()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                DoJob(hit);
            }
        }
    }

    public abstract void DoJob(RaycastHit hit);
}
