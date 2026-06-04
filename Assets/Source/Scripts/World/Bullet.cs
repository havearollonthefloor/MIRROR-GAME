using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : NetworkBehaviour
{
    [SyncVar] private float _speed;
    private Rigidbody2D _rb;
    private LayerMask _layerMask;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.up * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
        {
            return;
        }

        if ((_layerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (collision.gameObject.TryGetComponent(out Health enemy))
            {
                ServerDestroyEnemy(enemy);
            }
            else if (collision.gameObject.TryGetComponent(out Wall wall))
            {
                ServerDestroyBullet();
            }
        }
    }

    [Server]
    public void ServerInit(float speed, LayerMask enemy)
    {
        _speed = speed;
        _layerMask = enemy;
    }

    [Server]
    private void ServerDestroyEnemy(Health enemy)
    {
        enemy.TakeDamage(1);
        ServerDestroyBullet();
    }

    [Server]
    private void ServerDestroyBullet()
    {
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }
}