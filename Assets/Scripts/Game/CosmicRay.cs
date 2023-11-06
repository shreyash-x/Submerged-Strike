using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CosmicRay : MonoBehaviour
    {
        public float maxSpeed = 30;
        [SerializeField] private float visibleRange = 50;
    
        [SerializeField] private float maxStrength = 10;
        [SerializeField] private float splitDelay = 1; // split after every splitDelay seconds
        [SerializeField] private float splitProbability = 0.3f; // probability of splitting after every x second
        [SerializeField] private float minStrengthToSplit = 2; // minimum strength needed to split
        [SerializeField] private float maxDeflection = 20; // limit deflection current and spawned ray
        [SerializeField] private float minDeflection = 10; // minimum deflection
        [SerializeField] private float maxDepth = 3; // max recursive depth of spawning, limit rays spawned in unusual cases
    
        [SerializeField] private CosmicRay prefab;

        public Pool<CosmicRay> Pool;

        private Transform _camera;
        private TrailRenderer _trail;
        private float _elapsed = 0;
        private float _depth = 0;
        private float _speed = 0;
        private Gradient _defGradient;
    
        public float strength = 0;

        private void OnEnable()
        {
            if (_depth == 0)
            {
                strength = maxStrength;
            }
        
            _trail = GetComponent<TrailRenderer>();
            _defGradient = _trail.colorGradient;
            SetStrength(strength);
            _trail.emitting = true;

            if (Camera.main is { }) _camera = Camera.main.transform;
        }

        private void OnDisable()
        {
            _depth = 0;
            _elapsed = 0;
            _trail.Clear();
            _trail.emitting = false;
        }

        private void Update()
        {
            transform.position += transform.up * Time.deltaTime * _speed;
            if (Vector3.Distance(transform.position, _camera.position) > visibleRange)
            {
                // Destroy(gameObject);
                Pool.Return(this);
                return;
            }

            if (strength <= minStrengthToSplit || _depth >= maxDepth) return;
        
            _elapsed += Time.deltaTime;
            if (!(_elapsed >= splitDelay)) return;
            _elapsed = 0;
        
            var rng = Random.Range(0.0f, 1.0f);
            if (rng <= splitProbability)
            {
                // split
                var angle = Vector2.Angle(Vector2.up, transform.up);
                var rotation = Quaternion.AngleAxis(angle + GetDeflection(maxDeflection, minDeflection), Vector3.forward);
                var go = Pool.Borrow(false);
                go.transform.position = transform.position;
                go.transform.rotation = rotation;
                go.Pool = Pool;
                go._depth = _depth + 1;
                go.strength = Random.Range(strength / 7, strength * 6 / 7);
                go.gameObject.SetActive(true);
                SetStrength(strength - go.strength);
                transform.rotation = Quaternion.AngleAxis(angle + GetDeflection(minDeflection, 0), Vector3.forward);
            }
        }

        private void SetStrength(float s)
        {
            strength = s;
        
            _trail.widthMultiplier = strength / maxStrength;

            _trail.colorGradient = _defGradient;
            var colorGradient = _trail.colorGradient;
            for (int i = 0; i < colorGradient.alphaKeys.Length; i++)
            {
                colorGradient.alphaKeys[i].alpha *= strength / maxStrength;
            }
            _trail.colorGradient = colorGradient;
        
            _speed = maxSpeed * strength / maxStrength;
        }

        private static float GetDeflection(float maxDeflection, float minDeflection)
        {
            float delta = maxDeflection - minDeflection;
            delta = Random.Range(-delta, delta);
            if (delta < 0) return delta - minDeflection;
            return delta + minDeflection;
        }
    }
}
