using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Missiles
{
    public class MissileSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnDistance = 50;
        [SerializeField] private float spawnDelay = 1; // wait this much sec before spawning new
        [SerializeField] private MissileBasic prefab;

        internal GameObject Player;

        public IReadOnlyList<MissileBasic> ActiveMissiles => _pool.Active;

        private Pool<MissileBasic> _pool;

        private void Awake()
        {
            _pool = new Pool<MissileBasic>(prefab, 10);
        }

        public void StartSpawner()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            yield return new WaitForSeconds(spawnDelay);
            while (true)
            {
                if (Player == null) break;
                
                var position = Player.transform.position.xy() + Random.insideUnitCircle.normalized * spawnDistance;
                var missile = _pool.Borrow(false);
                missile.transform.position = position;
                missile.Pool = _pool;
                missile.gameObject.SetActive(true);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}