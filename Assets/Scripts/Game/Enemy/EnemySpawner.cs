using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    [System.Serializable]
    public struct EnemyProbabilityPair
    {
        public EnemyBase enemy;
        public float probability;
    }
    
    [System.Serializable]
    public struct WaveData
    {
        public float waveDuration;
        public float spawnInterval;
        public List<EnemyProbabilityPair> enemies;
    }
    

    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private WaveData[] waves;

        internal GameObject Player;

        public event Action<int> WaveStart;
        public event Action<int> WaveEnd;

        private readonly Dictionary<GameObject, Pool<EnemyBase>> _pools = new Dictionary<GameObject, Pool<EnemyBase>>();
        
        private readonly List<GameObject> _active = new List<GameObject>();
        public IReadOnlyList<GameObject> Active => _active;

        private int _currentWave;
        private float _elapsedSinceWaveStart, _elapsedSinceLastSpawn;

        private void OnEnable()
        {
            WaveStart?.Invoke(_currentWave);
        }

        private void Update()
        {
            _elapsedSinceWaveStart += Time.deltaTime;
            if (_elapsedSinceWaveStart >= waves[_currentWave].waveDuration)
            {
                if(_currentWave != waves.Length - 1) WaveEnd?.Invoke(_currentWave);
                _elapsedSinceWaveStart = 0;
                _currentWave++;
                if (_currentWave >= waves.Length) _currentWave = waves.Length - 1;
                else WaveStart?.Invoke(_currentWave);
            }

            _elapsedSinceLastSpawn += Time.deltaTime;
            if (_elapsedSinceLastSpawn >= waves[_currentWave].spawnInterval)
            {
                _elapsedSinceLastSpawn = 0;
                // spawn
                var enemyBase = SelectRandomEnemy(waves[_currentWave].enemies);
                var pool = GetPool(enemyBase);
                var enemy = pool.Borrow(false);
                enemy.Player = Player;
                enemy.Pool = pool;
                // enemy.EnemySpawner = this;
                enemy.Friendlies = _active;
                enemy.GetPool = GetPool;
                enemy.gameObject.SetActive(true);
            }
        }

        public Pool<EnemyBase> GetPool(EnemyBase enemyBase)
        {
            if (_pools.ContainsKey(enemyBase.gameObject))
            {
                return _pools[enemyBase.gameObject];
            }

            var pool = new Pool<EnemyBase>(enemyBase, 2);
            pool.Borrowed += (enemy) => _active.Add(enemy.gameObject);
            pool.Returned += (enemy) => _active.Remove(enemy.gameObject);
            _pools[enemyBase.gameObject] = pool;
            return pool;
        }

        private static EnemyBase SelectRandomEnemy(IEnumerable<EnemyProbabilityPair> enemies)
        {
            float random = Random.Range(0f, 1f);
            float sum = 0;
            foreach (var enemy in enemies)
            {
                sum += enemy.probability;
                if (sum >= random)
                {
                    return enemy.enemy;
                }
            }

            return null;
        }
    }
}