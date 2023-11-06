using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class MovementTutorial : TutorialStep
    {
        [System.Serializable]
        private class UI
        {
            public CanvasGroup root;
            public TextMeshProUGUI text;
            public Image screen;
            public Image arrow;
        }
        
        [SerializeField] private UI portraitUI;
        [SerializeField] private UI landscapeUI;
        [SerializeField] private float fadeDur = 1;

        [Space]
        [SerializeField] private InputManager inputManager;

        private static bool IsPortrait => Screen.height >= Screen.width;
        
        private bool _isActive;
        private bool _isEnding;
        private float _timeSinceEndStart;
        private float _timeSinceTutStart;

        public override void Begin()
        {
            _isActive = true;

            if (IsPortrait)
            {
                portraitUI.root.gameObject.SetActive(true);
            }
            else
            {
                landscapeUI.root.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (!_isActive) return;

            if (_isEnding)
            {
                _timeSinceEndStart += Time.unscaledDeltaTime;
                if (_timeSinceEndStart >= fadeDur)
                {
                    End();
                    return;
                }

                float alpha = 1 - (_timeSinceEndStart / fadeDur);

                UI ui = IsPortrait ? portraitUI : landscapeUI;
                ui.root.alpha = alpha;
                // Color color = new Color(1, 1, 1, alpha);
                // ui.arrow.color *= color;
                // ui.screen.color *= color;
                // ui.text.color *= color;
            }
            else
            {
                _timeSinceTutStart += Time.unscaledDeltaTime;
                float alpha = _timeSinceTutStart / fadeDur;
                if (alpha >= 1) alpha = 1;
                UI ui = IsPortrait ? portraitUI : landscapeUI;
                ui.root.alpha = alpha;
                
                // end and move to next tutorial stage as soon as player uses the joystick
                if (inputManager.GetMoveDirection() != Vector2.zero) _isEnding = true;
            }
        }

        private void End()
        {
            _isActive = _isEnding = false;
            (IsPortrait ? portraitUI : landscapeUI).root.gameObject.SetActive(false);
            OnEnd();
        }
    }
}
