using UnityEngine;
using Mirror;

public class PlayerRevive : NetworkBehaviour
{
    [SerializeField] private EnemyPool _pool;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdRevive();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdRevive()
    {
        _pool.ReviveAllMonsters();
    }
}
