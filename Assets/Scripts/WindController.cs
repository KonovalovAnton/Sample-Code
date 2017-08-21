using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {

    static public float WindAngle = 0;
    static public float WindSpeed = 1;
    static public float ANGLE_DELTA = 25;
    static public float WIND_MAX_SPEED = 25;

    [SerializeField]
    UnityEngine.UI.Slider _sliderSpeed;

    [SerializeField]
    UnityEngine.UI.Slider _sliderDirection;

    public void OnSliderSpeedMove()
    {
        WindSpeed = _sliderSpeed.value;
    }

    public void OnSliderDirectionMove()
    {
        WindAngle = _sliderDirection.value;
    }

}
