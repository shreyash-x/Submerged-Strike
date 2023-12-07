using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Narrative
{
    public class LevelNarrative : NarrativeStep
    {
        [System.Serializable]
        private class UI
        {
            public CanvasGroup root;
            public UITextTypeWriter tm_pro;
            public List<string> texts;
            public List<GameObject> imagesToDisplay;
            public Button button;
        }

        [SerializeField] private UI landscapeUI;
        [SerializeField] private float fadeDur = 1;

        [Space]

        private bool _isActive;
        private bool _isEnding;
        private float _timeSinceEndStart;
        private float _timeSinceTutStart;
        private string _currentText;
        private int _currentStep = -1;
        private GameObject prevImage;

        public override void Begin()
        {
            _isActive = true;
            landscapeUI.root.gameObject.SetActive(true);
            landscapeUI.button.onClick.AddListener(nextText);

            foreach (var image in landscapeUI.imagesToDisplay)
            {
                if (image != null)
                    image.SetActive(false);
            }

            nextText();
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

        private void nextText()
        {
            _currentStep++;
            if (prevImage != null)
                prevImage.SetActive(false);
            if (_currentStep >= landscapeUI.texts.Count)
            {
                _isEnding = true;
                return;
            }
            _currentText = landscapeUI.texts[_currentStep];
            prevImage = landscapeUI.imagesToDisplay[_currentStep];
            landscapeUI.tm_pro.image = prevImage;
            landscapeUI.tm_pro.SetNarrativeText(_currentText);
            landscapeUI.button.gameObject.SetActive(false);
        }

        private void End()
        {
            _isActive = _isEnding = false;
            Time.timeScale = 1.0f;
            OnEnd();
        }
    }
}
