using UnityEngine;
using Mirror;

public class PlayerInput : NetworkBehaviour
{
    private Vector2 _input;
    private bool _isShooting;
    private void Update()
    {
        if (!isLocalPlayer) return;

        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _isShooting = Input.GetKeyDown(KeyCode.Mouse0);
    }

    public Vector2 GetInput() => _input;

    public bool IsShooting()
    {
        if (_isShooting)
        {
            _isShooting = false;

            return true;
        }
        else
        {
            return false;
        }
    }
}
