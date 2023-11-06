using System.Collections;
using Game.Data;
using Game.Player;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private ResponsiveGameUIManager uiManager;
        [SerializeField] private Coin coinPrefab;
        [SerializeField] private float spawnDelay;
        [SerializeField] private float spawnDistance;

        internal PlayerController PlayerController;
        internal DataManager DataManager;

        private Pool<Coin> _pool;

        public void StartSpawning()
        {
            _pool = new Pool<Coin>(coinPrefab, 5);
            uiManager.GameUI.SetCoins(DataManager.CoinsCollected);
            PlayerController.CoinHit += () =>
            {
                DataManager.CoinsCollected++;
                uiManager.GameUI.SetCoins(DataManager.CoinsCollected);
            };
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                if (PlayerController == null) break;
                
                var position = PlayerController.transform.position.xy() + Random.insideUnitCircle.normalized * spawnDistance;
                var coin = _pool.Borrow(false);
                coin.transform.position = position;
                coin.Pool = _pool;
                coin.gameObject.SetActive(true);
                coin.Player = PlayerController.gameObject;
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}