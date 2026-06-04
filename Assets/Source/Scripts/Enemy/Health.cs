using UnityEngine;
using Mirror;
using System;
public class Health : NetworkBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private TeamManager _teamManager;

    [SyncVar (hook = "Invoker")] private int health;

    public event Action<float> OnHPChanged;

    public float HPPercent => (float)health / _maxHealth;

    public override void OnStartServer()
    {
        health = _maxHealth;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            if (_teamManager != null)
            {
                RoundManager.Instance.OnPlayerDeath(_teamManager.connectionToClient, _teamManager.Team);
            }

            health = 0;

            NetworkServer.UnSpawn(gameObject);
            gameObject.SetActive(false);
        }

        OnHPChanged?.Invoke(HPPercent);
    }

    private void Invoker(int old, int newValue)
    {
        OnHPChanged?.Invoke(HPPercent);
    }
}
