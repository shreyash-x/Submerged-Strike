using System;
using System.Collections;
using Game.Enemy;
using Game.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CosmicRaySpawner : MonoBehaviour
    {
        [SerializeField] private float spawnDistance = 50;
        [SerializeField] private float spawnDelay = 1; // wait this much sec before spawning new
        [SerializeField] private CosmicRay enemyPrefab;
        [SerializeField] private CosmicRay playerPrefab;

        internal PlayerController Player;
        internal EnemySpawner EnemySpawner;
        
        private Pool<CosmicRay> _enemyPool;
        private Pool<CosmicRay> _playerPool;

        private void Awake()
        {
            _enemyPool = new Pool<CosmicRay>(enemyPrefab, 10);
            _playerPool = new Pool<CosmicRay>(playerPrefab, 10);
        }

        public void StartSpawner()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                if (Player == null) break;
                
                var target = Player.GetComponent<Rigidbody2D>();
                var rand = Random.value;
                if (rand >= 0.5 && EnemySpawner.Active.Count > 0)
                {
                    target = EnemySpawner.Active.SelectRandom().GetComponent<Rigidbody2D>();
                }

                var currentTargetPosition = target.transform.position;
                var targetPosInFuture = (currentTargetPosition.xy() + target.velocity * (spawnDistance / enemyPrefab.maxSpeed));
                var position = currentTargetPosition.xy() + Random.insideUnitCircle.normalized * spawnDistance;
                var lookDir = targetPosInFuture - position;
                var ray = _enemyPool.Borrow(false);
                ray.transform.position = position;
                ray.transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir.normalized);
                ray.Pool = _enemyPool;
                ray.gameObject.SetActive(true);
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        public void ShootCosmicRayFromPlayer()
        {
            // Shoot a cosmic ray in the opposite direction of the player
            var position = Player.transform.position.xy();
            var lookDir = -Player.transform.up;
            var ray = _playerPool.Borrow(false);
            ray.transform.position = position + (Vector2)lookDir * 1.5f;
            ray.transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir.normalized);
            ray.Pool = _playerPool;
            ray.gameObject.SetActive(true);
        }
    }
}