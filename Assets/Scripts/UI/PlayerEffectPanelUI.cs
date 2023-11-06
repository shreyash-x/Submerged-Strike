using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerEffectPanelUI : MonoBehaviour
    {
        [SerializeField] private Image timeLeftIndicator;

        private float _duration = 0;
        private float _elapsed = 0;

        public void ShowForSeconds(float seconds)
        {
            gameObject.SetActive(true);
            _duration = Mathf.Max(seconds, _duration - _elapsed);
            _elapsed = 0;
        }

        public void ResetAndHide()

        {
            _elapsed = _duration = 0;
            timeLeftIndicator.fillAmount = 1;
            gameObject.SetActive(false);
        }
        private void OnDisable()
        {
            _elapsed = _duration = 0;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            timeLeftIndicator.fillAmount = 1 - (_elapsed / _duration);
            if (_elapsed >= _duration)
            {
                gameObject.SetActive(false);
            }
        }
    }
}