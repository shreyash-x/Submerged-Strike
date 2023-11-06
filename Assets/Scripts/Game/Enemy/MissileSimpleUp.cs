using System.Collections.Generic;
using Game.ModificationSystem;
using UnityEngine;

namespace Game.Enemy
{
    public class MissileSimpleUp : EnemyBase, IAllowXModification, IAllowYModification, IAllowSelfDestructModification
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private float randomLifeTimeDelta;
        [SerializeField] private float dieAnimationTime;
        [SerializeField] private float spawnDistance;
        [SerializeField] private float noModificationProbability = 0.3f;
        [SerializeField] private RigidbodyController controller;
        [SerializeField] private TrailRenderer trail;
        
        public float XMultiplier { get; set; }
        public float YMultiplier { get; set; }

        internal GameObject MissileShip { get; set; } = null;

        private Rigidbody2D _rigidbody;
        private float _elapsed;
        private float _lifeTime;
        private Vector3 _defaultScale;
        private bool _dead;

        public override bool IsActive() => !_dead && gameObject.activeSelf;
        
        private readonly List<(float probablity, IModification<MissileSimpleUp> modification)> _modifications = new List<(float probablity, IModification<MissileSimpleUp>)>()
        {
            (0.4f, new InvertXModification()),
            (0.4f, new InvertYModification()),
            (0.2f, new SelfDestructModification())
        };

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            controller.rigidbody = _rigidbody;
            _defaultScale = transform.localScale;
        }

        private void OnEnable()
        {
            if (Player == null) return;

            transform.localScale = _defaultScale;
            trail.emitting = true;
            trail.widthMultiplier = 1;
            _rigidbody.isKinematic = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            
            GetComponent<Collider2D>().enabled = true;
            _lifeTime = lifeTime + Random.Range(-randomLifeTimeDelta, randomLifeTimeDelta);
            controller.Init(transform.up);
        }
        
        private void OnDisable()
        {
            trail.Clear();
            trail.emitting = false;
            GetComponent<Collider2D>().enabled = false;
            _rigidbody.isKinematic = true;
            transform.localScale = _defaultScale;
            foreach (var modification in _modifications)
            {
                modification.modification.ResetOn(this);
            }
        }

        public void SelfDestruct()
        {
            Pool.Return(this);
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
                // _icon.SetActive(false);
                //
                _dead = true;
                if(elapsed >= dieAnimationTime) Pool.Return(this);
            }
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
            controller.Update(dir);
            
            // PositionIcon();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) return;
            if (other.gameObject == MissileShip) return;
            
            if (other.IsPartOfLayer("CosmicRay"))
            {
                var random = Random.Range(0f, 1f);
                if (random <= noModificationProbability)
                {
                    foreach (var modification in _modifications)
                    {
                        modification.modification.ResetOn(this);
                    }
                }
                else
                {
                    _modifications.SelectRandomP().ApplyOn(this);
                }
            } else if (other.IsPartOfLayer("Missile"))
            {
                // play mini explosion
                // if (_playDeadExplosion)
                // {
                //     other.GetComponent<MissileBasic>()._playDeadExplosion = false;
                //     var go = Instantiate(explosion);
                //     go.ExplodeAt(transform.position);
                // }
                Pool.Return(this);
            } else if(other.IsPartOfLayer("Player"))
            {
                Pool.Return(this);
            }
        }
    }
}