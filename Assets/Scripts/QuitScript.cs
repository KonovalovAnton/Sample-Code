using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Application exit only, do not work on editor
/// </summary>
public class QuitScript : MonoBehaviour {

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }
    
    /// <summary>
    /// UI button hook
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
