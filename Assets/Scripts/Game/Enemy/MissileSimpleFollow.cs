using System.Collections.Generic;
using Game.ModificationSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    [RequireComponent(typeof(Icon))]
    public class MissileSimpleFollow : EnemyBase, IAllowXModification, IAllowYModification, IAllowAttackFriendlyModification, IAllowSelfDestructModification
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private float randomLifeTimeDelta;
        [SerializeField] private float dieAnimationTime;
        [SerializeField] private float spawnDistance;
        [SerializeField] private float spawnDegree;
        [SerializeField] private float noModificationProbability = 0.3f;
        [SerializeField] private RigidbodyController controller;
        [SerializeField] private TrailRenderer trail;
        
        public GameObject Target { get; set; }

        public float XMultiplier { get; set; } = 1;
        public float YMultiplier { get; set; } = 1;

        private Rigidbody2D _rigidbody;
        
        private float _elapsed;
        private float _lifeTime;
        private Vector3 _defaultScale;
        private bool _dead;
        
        public override bool IsActive() => !_dead && gameObject.activeSelf;

        private readonly List<(float probablity, IModification<MissileSimpleFollow> modification)> _modifications = new List<(float probablity, IModification<MissileSimpleFollow>)>()
        {
            (0.3f, new InvertXModification()),
            (0.3f, new InvertYModification()),
            (0.3f, new AttackFriendlyModification()),
            (0.1f, new SelfDestructModification())
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
            
            _lifeTime = lifeTime + Random.Range(-randomLifeTimeDelta, randomLifeTimeDelta);
            Target = Player;
            // transform.position = Player.transform.position.xy() + Random.insideUnitCircle.normalized * spawnDistance;
            
            var angle = Random.Range(-spawnDegree, spawnDegree);
            var dir = Quaternion.AngleAxis(angle, Vector3.forward) * -Player.transform.up;
            transform.position = Player.transform.position + dir * spawnDistance;
        }

        private void OnDisable()
        {
            _elapsed = 0;
            _dead = false;
            showExplosion = true;
            transform.localScale = _defaultScale;
            trail.widthMultiplier = 1;
            foreach (var modification in _modifications)
            {
                modification.modification.ResetOn(this);
            }
        }

        public void SelfDestruct()
        {
            var go = Instantiate(explosion);
            go.ExplodeAt(transform.position);
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
            if (Target != null)
            {
                dir = (Target.transform.position - transform.position).normalized;
                dir.x *= XMultiplier;
                dir.y *= YMultiplier;
            }
            controller.Update(dir);
        }
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
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
                if (showExplosion)
                {
                    other.GetComponent<EnemyBase>().showExplosion = false;
                    var go = Instantiate(explosion);
                    go.ExplodeAt(transform.position);
                }
                Pool.Return(this);
            } else if(other.IsPartOfLayer("Player"))
            {
                Pool.Return(this);
            }
        }
    }
}