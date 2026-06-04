using UnityEngine;
using Mirror;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayersMovement : NetworkBehaviour
{
    [SerializeField] private float _speed;

    [SerializeField] private float _rotationSpeed = 120f;

    private PlayerInput _input;
    private Rigidbody2D _rb;
    private float _targetAngle;

    [SerializeField] private float _drag = 0.8f;
    [SerializeField] private float _verticalDrag = 0.6f;

    private Vector2 _currentVelocity;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();

        StartRotate();
    }

    private void StartRotate()
    {
        if (transform.position.y > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - transform.position).normalized;
        _targetAngle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90f;
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        Vector2 input = Vector2.ClampMagnitude(_input.GetInput(), 1f);
        Vector2 targetVelocity = input * _speed;

        _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, targetVelocity.x, 1f - _drag);
        _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, targetVelocity.y, 1f - _verticalDrag);

        _rb.linearVelocity = _currentVelocity;

        float delta = Mathf.DeltaAngle(transform.eulerAngles.z, _targetAngle);
        float maxDelta = _rotationSpeed * Time.fixedDeltaTime;
        float newAngularVelocity = delta / Time.fixedDeltaTime;
        newAngularVelocity = Mathf.Clamp(newAngularVelocity, -_rotationSpeed, _rotationSpeed);
        _rb.angularVelocity = newAngularVelocity;
    }
}
