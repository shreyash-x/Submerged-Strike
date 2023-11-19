using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class HUDTutorial : TutorialStep
    {
        [System.Serializable]
        private class UI
        {
            public CanvasGroup root;
            public List<GameObject> canvasGroups;
            public List<Button> buttons;
        }

        [SerializeField] private UI landscapeUI;
        [SerializeField] private float fadeDur = 1;

        [Space]

        private bool _isActive;
        private bool _isEnding;
        private float _timeSinceEndStart;
        private float _timeSinceTutStart;
        private GameObject _currentCanvasGroup;
        private int _currentStep = -1;

        public override void Begin()
        {
            _isActive = true;
            landscapeUI.root.gameObject.SetActive(true);
            // Set all gameobjects to inactive
            foreach (var canvasGroup in landscapeUI.canvasGroups)
            {
                canvasGroup.SetActive(false);
            }
            nextCanvasGroup();
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
            }
        }

        private void nextCanvasGroup()
        {
            _currentStep++;
            if (_currentStep >= landscapeUI.canvasGroups.Count)
            {
                _isEnding = true;
                return;
            }
            if (_currentCanvasGroup != null) _currentCanvasGroup.SetActive(false);
            _currentCanvasGroup = landscapeUI.canvasGroups[_currentStep];
            _currentCanvasGroup.SetActive(true);
            var currentButton = landscapeUI.buttons[_currentStep];
            currentButton.onClick.AddListener(nextCanvasGroup);
        }

        private void End()
        {
            _isActive = _isEnding = false;
            _currentCanvasGroup.SetActive(false);
            Time.timeScale = 1.0f;
            OnEnd();
        }
    }
}
