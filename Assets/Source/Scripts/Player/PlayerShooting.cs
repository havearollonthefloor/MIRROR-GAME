using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerInput))]
public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private float _bulletSpeed;

    [SerializeField] private LayerMask _blueLayerMask;
    [SerializeField] private LayerMask _redLayerMask;

    [SerializeField] private TeamManager _manager;

    private Teams _myTeam => _manager.Team;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (_playerInput.IsShooting())
        {
            CmdShoot();
        }
    }

    [Command]
    private void CmdShoot()
    {
        GameObject bullet = Instantiate(_bulletPrefab, _firePosition.position, _firePosition.rotation);

        switch (_myTeam)
        {
            case Teams.Blue:
                bullet.GetComponent<Bullet>().ServerInit(_bulletSpeed, _redLayerMask);
                break;
            case Teams.Red:
                bullet.GetComponent<Bullet>().ServerInit(_bulletSpeed, _blueLayerMask);
                break;
        }

        NetworkServer.Spawn(bullet);
    }
}
