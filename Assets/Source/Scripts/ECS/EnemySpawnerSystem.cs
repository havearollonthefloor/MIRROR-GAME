using Leopotam.Ecs;
using Mirror;
using UnityEngine;

namespace Source.Ecs
{
    public struct SpawnerComponent
    {

    }
    public class EnemySpawnerSystem : IEcsInitSystem
    {
        private readonly EcsWorld _world = null;
        private GameObject _enemyPrefab = null;
        private Vector3[] _spawnPoints = null;

        [Command]
        public void Init()
        {
            if (!NetworkServer.active) return;

            foreach (var spawnPoint in _spawnPoints)
            {
                var entity = _world.NewEntity();

                entity.Get<EnemyTag>();
                entity.Get<MovableComponent>();
                ref var position = ref entity.Get<PositionComponent>();

                position.Position = spawnPoint;

                var enemy = Object.Instantiate(_enemyPrefab, position.Position, Quaternion.identity);
                NetworkServer.Spawn(enemy);
            }
        }

        public void Run() { }
    }
}
