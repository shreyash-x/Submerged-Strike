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
    public struct MineCountPair
    {
        public EnemyBase mine;
        public int count;
    }
    
    [System.Serializable]
    public struct WaveData
    {
        public float waveDuration;
        public float spawnInterval;
        public List<EnemyProbabilityPair> enemies;
        public List<MineCountPair> mines;
        public int totalMinesCount;
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

        private int _spawnedMines = 0;

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
                _spawnedMines = 0;
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
                if (_spawnedMines < waves[_currentWave].totalMinesCount)
                {
                    var mineBase = SelectNextMine(waves[_currentWave].mines, _spawnedMines);
                    var minePool = GetPool(mineBase);
                    var mine = minePool.Borrow(false);
                    mine.Player = Player;
                    mine.Pool = minePool;
                    mine.Friendlies = _active;
                    mine.GetPool = GetPool;
                    mine.gameObject.SetActive(true);
                }
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

        private static EnemyBase SelectNextMine(IEnumerable<MineCountPair> mines, int spawnedMines)
        {
            int counter = spawnedMines;
            foreach (var mine in mines)
            {
                counter -= mine.count;

                if (counter < 0)
                {
                    return mine.mine;
                }
            }

            return null;
        }
    }
}