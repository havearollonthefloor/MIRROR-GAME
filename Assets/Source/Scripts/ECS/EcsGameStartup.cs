using UnityEngine;
using Leopotam.Ecs;
using Voody.UniLeo;

namespace Source.Ecs
{
    public class EcsGameStartup : MonoBehaviour
    {
        [SerializeField] private EnemyInitData _enemyInitData;
        [SerializeField] private Transform[] _spawnPoints;

        private EcsWorld _world;
        private EcsSystems _systems;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            AddInjections();
            AddOneFrames();
            AddSystems();

            _systems.ConvertScene();
            _systems.Init();
        }


        private void AddInjections()
        {

        }

        private void AddSystems()
        {
            Vector3[] spawnPositions = new Vector3[_spawnPoints.Length];
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                spawnPositions[i] = _spawnPoints[i].transform.position;
            }

            _systems.Add(new EnemySpawnerSystem()).Add(new EnemyMovementSystem())
            .Inject(_enemyInitData.EnemyPrefab)
            .Inject(spawnPositions);
        }

        private void AddOneFrames()
        {

        }

        private void Update()
        {
            _systems.Run();
        }

        private void OnDestroy()
        {
            if (_systems == null) return;

            _systems.Destroy();
            _systems = null;

            _world.Destroy();
            _world = null;
        }
    }
}
