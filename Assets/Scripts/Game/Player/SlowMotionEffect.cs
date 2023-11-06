using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Game.Player
{
    public class SlowMotionEffect : MonoBehaviour
    {
        [SerializeField] private PostProcessVolume vignetteVolume;
        [SerializeField] private float effectDuration;
        [SerializeField] private AnimationCurve vignetteWeightCurve;
        [SerializeField] private AnimationCurve timeScaleCurve;

        internal PlayerController PlayerController;

        private float _elapsed;
        private bool _transitioning;
        private bool _slowDownTime;

        private void Awake()
        {
            vignetteVolume.weight = 0;
        }

        private void Start()
        {
            PlayerController.CosmicRayHit += (controller, ray) =>
            {
                _slowDownTime = true;
                _transitioning = true;
                _elapsed = 0;
            };
            
            PlayerController.MissileHit += (controller, ray) =>
            {
                _slowDownTime = false;
                _transitioning = true;
                _elapsed = 0;
            };
        }

        private void Update()
        {
            if (!_transitioning) return;

            _elapsed += Time.deltaTime;

            vignetteVolume.weight = Mathf.Clamp01(vignetteWeightCurve.Evaluate(_elapsed / effectDuration));
            
            if(_slowDownTime) Time.timeScale = timeScaleCurve.Evaluate(_elapsed / effectDuration);

            if (_elapsed >= effectDuration)
            {
                if(_slowDownTime) Time.timeScale = 1;
                _transitioning = false;
                _elapsed = 0;
                vignetteVolume.weight = 0;
            }
        }
    }
}