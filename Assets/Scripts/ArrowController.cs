using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour {

    [SerializeField]
    Slider _direction;

	public void Rotate()
    {
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -_direction.value);
    }
}
