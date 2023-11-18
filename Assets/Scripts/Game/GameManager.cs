using System;
using System.Globalization;
using System.Collections;
using Cinemachine;
using Game.Data;
using Game.Enemy;
using Game.Objective;
using Game.Player;
using GameAnalyticsSDK;
using TMPro;
using Tutorial;
using UI;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Unity.VectorGraphics;


namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private DataManager dataManager;
        [SerializeField] private GameData gameData;

        [SerializeField] internal TutorialManager tutorialManager;

        [SerializeField] internal ModificationController modificationController;
        [SerializeField] internal InputManager inputManager;
        [SerializeField] internal GameObject backgroundPrefab;

        [SerializeField] internal EnemySpawner enemySpawner;
        [SerializeField] internal ObjectiveManager objectiveManager;

        // [SerializeField] internal MissileSpawner[] missileSpawners;
        [SerializeField] internal CosmicRaySpawner cosmicRaySpawner;
        [SerializeField] internal CoinManager coinManager;

        [SerializeField] internal CinemachineVirtualCamera cinemachine;

        [SerializeField] internal SlowMotionEffect slowMotionEffect;

        [SerializeField] internal ResponsiveGameUIManager uiManager;
        [SerializeField] internal Explosion deadExplosion;
        [SerializeField] private Button shootButton;

        internal PlayerController PlayerController;
        internal Player.Shield Shield;

        private float _playStartTime;

        private bool _playing = true;
        private int _currentPlayCoins = 0;

        private bool _isPlayingTutorial = false;

        private int _lastWave;
        private float _waveStartTime;

        private void Awake()
        {
            Time.timeScale = 1f;
            dataManager.gameData = gameData;
            var playerPrefab = gameData.planes[dataManager.EquippedPlane].planePrefab;
            var shieldPrefab = gameData.shields[dataManager.EquippedShield].shieldPrefab;

            PlayerController = Instantiate(playerPrefab);
            PlayerController.InputManager = inputManager;
            PlayerController.DeadExplosion = deadExplosion;

            Shield = Instantiate(shieldPrefab);

            var shieldGo = Shield.gameObject;
            var playerGo = PlayerController.gameObject;

            Shield.Player = playerGo;

            modificationController.PlayerController = PlayerController;
            modificationController.Shield = shieldGo;

            enemySpawner.Player = playerGo;
            enemySpawner.isIconsEnabled = dataManager.EnableIcons;
            enemySpawner.DataManager = dataManager;
            enemySpawner.UIManager = uiManager;

            objectiveManager.Player = playerGo;

            cosmicRaySpawner.Player = PlayerController;
            cosmicRaySpawner.EnemySpawner = enemySpawner;

            coinManager.PlayerController = PlayerController;
            coinManager.DataManager = dataManager;

            var playerTransform = PlayerController.transform;
            cinemachine.Follow = playerTransform;
            cinemachine.LookAt = playerTransform;

            slowMotionEffect.PlayerController = PlayerController;

            // if there are not zero that means the game crashed midway, find better way to display this
            dataManager.CoinsCollected = 0;
            dataManager.MinesDestroyed = 0;
            dataManager.TimeCoins = 0;

            PlayerController.MissileHit += (controller, _) =>
            {
                _playing = false;
                Destroy(PlayerController.gameObject);
                Destroy(Shield.gameObject);

                var ui = GameObject.FindObjectsOfType<Canvas>();
                foreach (var canvas in ui)
                {
                    canvas.gameObject.SetActive(false);
                }

                dataManager.TimeCoins = Mathf.FloorToInt(Time.time - _playStartTime);
                _currentPlayCoins = dataManager.CoinsCollected + dataManager.TimeCoins;

                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coins", dataManager.CoinsCollected, "Gameplay", "CoinsCollected");
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coins", dataManager.TimeCoins, "Gameplay", "TimeCoins");

                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail,
                    $"Plane_{dataManager.EquippedPlane}",
                    $"Shield_{dataManager.EquippedShield}",
                    $"Wave_{_lastWave}",
                    (int)(Time.time - _waveStartTime)
                );

                dataManager.Coins += _currentPlayCoins;
                uiManager.DeadMenu.Display(dataManager.TimeCoins, dataManager.CoinsCollected, dataManager.Coins);

                dataManager.CoinsCollected = 0;
                dataManager.MinesDestroyed = 0;
                dataManager.TimeCoins = 0;
                PlayerController = null;
                Shield = null;
            };

            // NOTE works only when WaveEnd called before next WaveStart
            enemySpawner.WaveStart += (wave) =>
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start,
                    $"Plane_{dataManager.EquippedPlane}",
                    $"Shield_{dataManager.EquippedShield}",
                    $"Wave_{wave}"
                );
                _lastWave = wave;
                _waveStartTime = Time.time;
            };

            enemySpawner.WaveEnd += (wave) =>
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete,
                    $"Plane_{dataManager.EquippedPlane}",
                    $"Shield_{dataManager.EquippedShield}",
                    $"Wave_{wave}",
                    (int)(Time.time - _waveStartTime)
                );
            };


            shootButton.onClick.AddListener(() =>
            {
                // Shoot a cosmic ray in the opposite direction of the player
                cosmicRaySpawner.ShootCosmicRayFromPlayer();

                // Start coroutine to enable button after 5 seconds
                StartCoroutine(EnableButtonAfterSeconds(5, shootButton));
            });
        }

        private void Start()
        {
            inputManager.SetInputMode(dataManager.InputMode);
            _playing = true;

            if (dataManager.PlayTutorial)
            {
                dataManager.PlayTutorial = false;

                _isPlayingTutorial = true;
                tutorialManager.TutorialComplete += () =>
                {
                    _isPlayingTutorial = false;
                    _playStartTime = Time.time;
                    coinManager.StartSpawning();
                };
                tutorialManager.StartTutorial();
            }
            else
            {
                StartMissileSpawner();
                StartRaySpawner();
                StartObjectiveCreation();
                coinManager.StartSpawning();
                _playStartTime = Time.time;
            }
        }

        private void Update()
        {
            if (!_playing) return;

            AudioListener.volume = dataManager.EffectsVolume;
            var elapsed = Time.time - _playStartTime;
            if (!_isPlayingTutorial)
            {
                uiManager.GameUI.SetTime(TimeSpan.FromSeconds(elapsed)
                    .ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));

                uiManager.GameUI.ToggleRadar(dataManager.EnableRadar);
            }
        }

        public void StartMissileSpawner()
        {
            enemySpawner.enabled = true;
        }

        public void StartObjectiveCreation()
        {
            objectiveManager.enabled = true;
        }

        public void StartRaySpawner()
        {
            cosmicRaySpawner.StartSpawner();
        }

        private void OnAdSuccess()
        {
            dataManager.Coins += _currentPlayCoins;

            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coins", _currentPlayCoins, "Ads", "EndGameReward");

            _currentPlayCoins = 0;
            uiManager.DeadMenu.UpdateTotalCoins(dataManager.Coins);
        }

        private void OnDestroy()
        {
            dataManager = null;
            gameData = null;
            tutorialManager = null;
            modificationController = null;
            inputManager = null;
            // missileSpawners = null;
            enemySpawner = null;
            objectiveManager = null;
            cosmicRaySpawner = null;
            coinManager = null;
            cinemachine = null;
            slowMotionEffect = null;
            uiManager = null;
            PlayerController = null;
            Shield = null;
        }

        private IEnumerator EnableButtonAfterSeconds(int seconds, Button button)
        {
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            // Get SVG Image component in child
            var svgImage = button.GetComponentInChildren<SVGImage>();
            // Hide SVG Image
            svgImage.enabled = false;
            button.interactable = false;
            var time = seconds;
            while (time > 0)
            {
                text.text = time.ToString() + "s";
                yield return new WaitForSeconds(1);
                time--;
            }
            button.interactable = true;
            text.text = "";
            // Show SVG Image
            svgImage.enabled = true;
        }
    }
}