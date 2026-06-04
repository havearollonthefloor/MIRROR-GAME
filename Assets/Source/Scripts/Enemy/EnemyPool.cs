using UnityEngine;
using System.Collections.Generic;
using Mirror;
public class EnemyPool : NetworkBehaviour
{
    [SerializeField] private List<GameObject> _enemyPool;

    [Server]
    public void ReviveAllMonsters()
    {
        foreach (GameObject enemy in _enemyPool)
        {
            ServerRevive(enemy);
        }
    }

    [Server]

    private void ServerRevive(GameObject enemy)
    {
        NetworkServer.Spawn(enemy);
        enemy.SetActive(true);
    }
}
