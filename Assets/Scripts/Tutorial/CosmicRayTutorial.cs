using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class CosmicRayTutorial : TutorialStep
    {
        [System.Serializable]
        private class UI
        {
            public CanvasGroup root;
            public Image screen;
            public TextMeshProUGUI text;
            public Button okBtn;
            public GameObject mask;
        }

        [SerializeField] private UI portraitUI;
        [SerializeField] private UI landscapeUI;
        [SerializeField] private float arrowDistance = 5;
        [SerializeField] private float fadeDur = 2;
        
        private bool _isActive;
        private bool _isEnding;
        
        private Camera _camera;

        private UI _currentUI;
        private GameObject _ray;
        private bool _foundRay;

        private float _timeSinceFoundRay;
        private float _timeSinceEnd;

        private void Start()
        {
            _camera = Camera.main;
            _currentUI = Screen.height >= Screen.width ? portraitUI : landscapeUI;
            _currentUI.okBtn.onClick.AddListener(() => { _isEnding = true; });
        }

        public override void Begin()
        {
            gameManager.StartRaySpawner();
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
            if (!_foundRay)
            {
                var cosmicRay =
                    Physics2D.OverlapArea(screenBounds.min, screenBounds.max, LayerMask.GetMask("CosmicRay"));
                if (cosmicRay == null) return;

                _foundRay = true;
                _ray = cosmicRay.gameObject;
                _currentUI.root.gameObject.SetActive(true);
            }

            if (_foundRay)
            {
                _timeSinceFoundRay += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(_timeSinceFoundRay / fadeDur);
                Time.timeScale = Mathf.LerpAngle(1, 0.06f, 3 * t);
                _currentUI.root.alpha = t;
                _currentUI.mask.transform.position = _camera.WorldToScreenPoint(_ray.transform.position);
            }
        }

        private void End()
        {
            _currentUI.root.gameObject.SetActive(false);
            _foundRay = _isActive = _isEnding = false;
            _ray = null;
            _currentUI = null;
            Time.timeScale = 1.0f;
            OnEnd();
        }
    }
}