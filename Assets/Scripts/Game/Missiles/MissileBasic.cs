using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Missiles
{
    internal enum ModificationType
    {
        None,
        InvertX,
        InvertY,
        InvertXY,
        AttackFriendly,
        SelfDestruct
    }
    
    [Serializable]
    internal class Modification
    {
        public float probability;
        public ModificationType type;
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Missiles/Basic")]
    public class MissileBasic : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private float randomLifeTimeDelta;
        [SerializeField] private float dieAnimationTime;
        [SerializeField] private RigidbodyController controller;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private Modification[] modifications; // 0 must be none
        [SerializeField] private Explosion explosion;
        [SerializeField] private GameObject offScreenIcon;
        [SerializeField] private float screenBorder;

        private Rigidbody2D _rigidbody;
        private GameObject _player;
        private SpriteRenderer _renderer;
        private Animation _animation;
        
        // modification helpers
        private GameObject _target;
        private float _xMultiplier = 1;
        private float _yMultiplier = 1;

        private float _lifeTime;
        private float _elapsed;
        private ModificationType _currentModificationType;

        private Vector3 _defaultScale;
        private bool _dead = false;

        private bool _playDeadExplosion = true;

        private GameObject _icon;
        private Camera _camera;

        public Pool<MissileBasic> Pool;

        private void Awake()
        {
            _animation = GetComponent<Animation>();
            _renderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
            controller.rigidbody = _rigidbody;

            _camera = Camera.main;
            
            _icon = Instantiate(offScreenIcon);
            _icon.SetActive(false);

            _defaultScale = transform.localScale;
            _currentModificationType = ModificationType.None;
        }

        private void OnEnable()
        {
            _player = GameObject.FindWithTag("Player");
            _target = _player;
            
            _playDeadExplosion = true;
            _lifeTime = lifeTime + Random.Range(-1, 1) * randomLifeTimeDelta;
            _xMultiplier = _yMultiplier = 1;
            _elapsed = 0;
            _dead = false;
            _currentModificationType = ModificationType.None;
            transform.localScale = _defaultScale;
            trail.widthMultiplier = 1;
        }

        private void OnDisable()
        {
            if (_icon != null)
            {
                _icon.SetActive(false);
            }
            ApplyModification(ModificationType.None);
            _animation.Stop();
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _lifeTime)
            {
                var elapsed = _elapsed - _lifeTime;
                var t =  elapsed / dieAnimationTime;
                transform.localScale = _defaultScale * Mathf.Lerp(1, 0, t);
                trail.widthMultiplier = Mathf.Lerp(1, 0, t);
                _icon.SetActive(false);
                
                _dead = true;
                if(elapsed >= dieAnimationTime) Pool.Return(this);
            }
        }

        private void LateUpdate()
        {
            PositionIcon();
        }

        private void FixedUpdate()
        {
            if (_dead)
            {
                _rigidbody.drag = 0.5f;
                return;
            }
            _rigidbody.drag = 10;

            var dir = transform.up;
            if (_target != null)
            {
                dir = (_target.transform.position - transform.position).normalized;
                dir.x *= _xMultiplier;
                dir.y *= _yMultiplier;
            }
            controller.Update(dir);
            
            // PositionIcon();
        }

        private void PositionIcon()
        {
            if (!_dead && !_renderer.isVisible && _player != null)
            {
                if(!_icon.activeSelf) _icon.SetActive(true);
            
                _icon.transform.position = Utility.WorldPosToBorder(transform.position, screenBorder, _camera);
                _icon.transform.up = (_player.transform.position - _icon.transform.position).normalized;
            }
            else
            {
                if(_icon.activeSelf) _icon.SetActive(false);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.IsPartOfLayer("CosmicRay"))
            {
                if (_currentModificationType == ModificationType.None)
                {
                    var random = Random.Range(0f, 1f);
                    float sum = 0;
                    foreach (var modification in modifications)
                    {
                        sum += modification.probability;
                        if (random <= sum)
                        {
                            // apply modification
                            _currentModificationType = modification.type;
                            break;
                        }
                    }
                }
                else _currentModificationType = ModificationType.None;
                ApplyModification(_currentModificationType);
            } else if (other.IsPartOfLayer("Missile"))
            {
                // play mini explosion
                if (_playDeadExplosion)
                {
                    other.GetComponent<MissileBasic>()._playDeadExplosion = false;
                    var go = Instantiate(explosion);
                    go.ExplodeAt(transform.position);
                }
                Pool.Return(this);
            } else if(other.IsPartOfLayer("Player"))
            {
                Pool.Return(this);
            }
        }

        private void ApplyModification(ModificationType modificationType)
        {
            _animation.Stop();
            if (modificationType != ModificationType.None)
            {
                _animation.Play();
            }
            switch (modificationType)
            {
                case ModificationType.None:
                    _xMultiplier = _yMultiplier = 1;
                    _target = _player;
                    break;
                case ModificationType.InvertX:
                    _xMultiplier = -1;
                    break;
                case ModificationType.InvertY:
                    _yMultiplier = -1;
                    break;
                case ModificationType.InvertXY:
                    _xMultiplier = -1;
                    _yMultiplier = -1;
                    break;
                case ModificationType.AttackFriendly:
                    var active = Pool.Active;
                    if (active.Count == 0)
                    {
                        _target = null;
                    }
                    else
                    {
                        var random = Random.Range(0, active.Count);
                        _target = active[random].gameObject;
                    }
                    break;
                case ModificationType.SelfDestruct:
                    var go = Instantiate(explosion);
                    go.ExplodeAt(transform.position);
                    Pool.Return(this);
                    break;
                default:
                    Debug.LogError($"Unsupported Modification Type {modificationType}");
                    break;
            }
        }
    }
}