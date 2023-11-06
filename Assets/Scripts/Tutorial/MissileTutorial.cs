using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class MissileTutorial : TutorialStep
    {
        [System.Serializable]
        private class UI
        {
            public CanvasGroup root;
            public GameObject mask;
            public Button okBtn;
        }

        [SerializeField] private UI portraitUI;
        [SerializeField] private UI landscapeUI;
        [SerializeField] private float fadeDur;
        
        private UI _currentUI;
        private Camera _camera;
        private bool _isEnding;
        private bool _isActive;
        private float _timeSinceEnd;
        private bool _foundMissile;
        private GameObject _missile;
        private float _timeSinceFoundMissile;

        private void Awake()
        {
            _camera = Camera.main;
            _currentUI = Screen.height >= Screen.width ? portraitUI : landscapeUI;
            _currentUI.okBtn.onClick.AddListener(() => { _isEnding = true; });
        }

        public override void Begin()
        {
            gameManager.StartMissileSpawner();
            _isActive = true;
        }

        private void LateUpdate()
        {
            if (!_isActive) return;
            
            if (_isEnding)
            {
                _timeSinceEnd += Time.unscaledDeltaTime;
                float t = 1 - Mathf.Clamp01(_timeSinceEnd / fadeDur);
                Time.timeScale = 1 - t;
                if (_timeSinceEnd >= fadeDur)
                {
                    End();
                    return;
                }
                _currentUI.root.alpha = t;
                return;
            }

            var screenBounds = Utility.GetScreenBounds(10, _camera);
            if (!_foundMissile)
            {
                var missile =
                    Physics2D.OverlapArea(screenBounds.min, screenBounds.max, LayerMask.GetMask("Missile"));
                if (missile == null) return;

                _foundMissile = true;
                _missile = missile.gameObject;
                _currentUI.root.gameObject.SetActive(true);
            }

            if (_foundMissile)
            {
                Time.timeScale = 0.07f;
                _timeSinceFoundMissile += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(_timeSinceFoundMissile / fadeDur);
                _currentUI.root.alpha = t;
                _currentUI.mask.transform.position = _camera.WorldToScreenPoint(_missile.transform.position);
            }
        }

        private void End()
        {
            _currentUI.root.gameObject.SetActive(false);
            _foundMissile = _isActive = _isEnding = false;
            _missile = null;
            _currentUI = null;
            Time.timeScale = 1.0f;
            OnEnd();
        }
    }
}