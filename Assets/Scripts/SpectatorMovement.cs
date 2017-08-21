using UnityEngine;

/// <summary>
/// Simple free camera
/// </summary>
public class SpectatorMovement : MonoBehaviour {

    /// <summary>
    /// Fixes bug when long delta time during plants generation causes sudden camera jump
    /// </summary>
    private const float FROZE_TIME = 1.0f / 15;

    [SerializeField]
    private float _cameraSensitivity = 90;

    [SerializeField]
    private float _normalMoveSpeed = 200;

    private float _rotationX = 0.0f;
    private float _rotationY = 0.0f;

    private bool _active = true;

    /// <summary>
    /// Fixes bug when initial camera position is ignored and overridden with the first zero input. Causes sudden camera jump
    /// </summary>
    private Quaternion _startRot;

    private void Start()
    {
        _startRot = transform.rotation;
    }

    void Update()
    {
        if(CheckActive())
        {
            Rotate();
            Move();
        }
    }

    private bool CheckActive()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _active = !_active;
        }
        return _active;
    }

    private void Rotate()
    {
        if (Time.deltaTime < FROZE_TIME)
        {
            _rotationX += Input.GetAxis("Mouse X") * _cameraSensitivity * Time.deltaTime;
            _rotationY += Input.GetAxis("Mouse Y") * _cameraSensitivity * Time.deltaTime;
            _rotationY = Mathf.Clamp(_rotationY, -90, 90);

            var quat = Quaternion.AngleAxis(_rotationX, Vector3.up);
            quat *= Quaternion.AngleAxis(_rotationY, Vector3.left);
            transform.rotation = quat * _startRot;
        }
    }

    private void Move()
    {
        float horizontal = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        float vertical = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;
        transform.localPosition += vertical * transform.forward * _normalMoveSpeed * Time.deltaTime;
        transform.localPosition += horizontal * transform.right * _normalMoveSpeed * Time.deltaTime;
    }
}
