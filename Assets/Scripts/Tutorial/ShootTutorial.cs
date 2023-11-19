using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class ShootingTutorial : TutorialStep
    {
        [System.Serializable]
        private class UI
        {
            public CanvasGroup root;
            public TextMeshProUGUI text;
            public Image screen;
            public Image arrow;
            public Button okBtn;
            public GameObject mask;
        }

        [SerializeField] private UI landscapeUI;
        [SerializeField] private float fadeDur = 1;

        [Space]
        [SerializeField] private Button shootButton;

        private bool _isActive;
        private bool _isEnding;
        private float _timeSinceEndStart;
        private float _timeSinceTutStart;
        private bool _foundRay;
        private GameObject _ray;
        private Camera _camera;
        private float _timeSinceFoundRay;

        public override void Begin()
        {
            _isActive = true;
            _camera = Camera.main;
            landscapeUI.root.gameObject.SetActive(true);
            // Hide ok button until player shoots
            landscapeUI.okBtn.gameObject.SetActive(false);

            shootButton.interactable = true;
            shootButton.onClick.AddListener(() =>
            {
                landscapeUI.okBtn.gameObject.SetActive(true);
            });
        }

        private void LateUpdate()
        {
            UI ui = landscapeUI;
            if (!_isActive) return;

            if (_isEnding)
            {
                _timeSinceEndStart += Time.unscaledDeltaTime;
                float t = 1 - Mathf.Clamp01(_timeSinceEndStart / fadeDur);
                Time.timeScale = 1 - t;
                if (_timeSinceEndStart >= fadeDur)
                {
                    End();
                    return;
                }
                ui.root.alpha = t;
                return;
            }
            else
            {
                _timeSinceTutStart += Time.unscaledDeltaTime;
                float alpha = _timeSinceTutStart / fadeDur;
                if (alpha >= 1) alpha = 1;
                ui.root.alpha = alpha;

                // // end and move to next tutorial stage as soon as player uses the joystick
                // if (inputManager.GetMoveDirection() != Vector2.zero) _isEnding = true;
            }
            var screenBounds = Utility.GetScreenBounds(10, _camera);
            if (!_foundRay)
            {
                var cosmicRay =
                    Physics2D.OverlapArea(screenBounds.min, screenBounds.max, LayerMask.GetMask("CosmicRay"));
                if (cosmicRay == null) return;

                _foundRay = true;
                _ray = cosmicRay.gameObject;
                ui.text.text = "Good job! You can see the cosmic ray you just shot on the screen.";
                ui.arrow.gameObject.SetActive(false);
                var prevMask = ui.mask.transform.position;
                ui.okBtn.onClick.AddListener(() =>
                {
                    ui.text.text = "The shoot button goes into cooldown for a few seconds after you use it.";
                    ui.okBtn.onClick.RemoveAllListeners();
                    ui.mask.transform.position = prevMask;
                    ui.arrow.gameObject.SetActive(true);
                    ui.okBtn.onClick.AddListener(() => { _isEnding = true; });
                });

            }

            if (_foundRay)
            {
                _timeSinceFoundRay += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(_timeSinceFoundRay / fadeDur);
                Time.timeScale = Mathf.LerpAngle(1, 0.06f, 4 * t);
                ui.root.alpha = t;
                ui.mask.transform.position = _camera.WorldToScreenPoint(_ray.transform.position);
            }
        }

        private void End()
        {
            _isActive = _isEnding = false;
            landscapeUI.root.gameObject.SetActive(false);
            _foundRay = _isActive = _isEnding = false;
            _ray = null;
            Time.timeScale = 1.0f;
            OnEnd();
        }
    }
}
