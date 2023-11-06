using System.Collections.Generic;
using Game.ModificationSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    [RequireComponent(typeof(Icon))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MissileDual : EnemyBase, IAllowXModification, IAllowYModification, IAllowSelfDestructModification
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private float minSplitDistance;
        [SerializeField] private float spawnDistance;
        [SerializeField] private float spawnDegree; // degrees on both sides of direction opp of player up to spawn missile
        [SerializeField] private float noModificationProbability;
        
        [SerializeField] private RigidbodyController controller;
        [SerializeField] private MissileDualPart[] parts;
        [SerializeField] private TrailRenderer trail;

        private Vector3[] _partsLocationPosition;
        private float _elapsed;
        private bool _split;

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

        public float XMultiplier { get; set; } = 1;
        public float YMultiplier { get; set; } = 1;

        public override bool IsActive() => !_split && gameObject.activeSelf;
        
        private readonly List<(float probablity, IModification<MissileDual> modification)> _modifications = new List<(float probablity, IModification<MissileDual>)>()
        {
            (0.4f, new InvertXModification()),
            (0.4f, new InvertYModification()),
            (0.2f, new SelfDestructModification())
        };

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _partsLocationPosition = new[]
            {
                parts[0].transform.localPosition,
                parts[1].transform.localPosition
            };
        }
        
        public void SelfDestruct()
        {
            var go = Instantiate(explosion);
            go.ExplodeAt(transform.position);
            Pool.Return(this);
        }

        private void OnEnable()
        {
            if (Player == null) return;
            
            // transform.position = Player.transform.position.xy() + Random.insideUnitCircle.normalized * spawnDistance;
            var angle = Random.Range(-spawnDegree, spawnDegree);
            var dir = Quaternion.AngleAxis(angle, Vector3.forward) * -Player.transform.up;
            transform.position = Player.transform.position + dir * spawnDistance;
            
            parts[0].transform.SetParent(transform);
            parts[1].transform.SetParent(transform);
            parts[0].transform.localPosition = _partsLocationPosition[0];
            parts[1].transform.localPosition = _partsLocationPosition[1];
            
            parts[0].Player = Player;
            parts[1].Player = Player;
            
            parts[0].gameObject.SetActive(true);
            parts[1].gameObject.SetActive(true);

            _collider.enabled = true;
            _rigidbody.simulated = true;
            controller.rigidbody = _rigidbody;

            // _spriteRenderer.color = Color.white;
            if(trail != null) trail.emitting = true;
        }

        private void OnDisable()
        {
            // Destroy(_rigidbody);
            _rigidbody.simulated = false;
            _collider.enabled = false;
            _split = showExplosion = false;
            _elapsed = 0;
            if(trail != null) trail.emitting = false;
            
            showExplosion = true;
            
            foreach (var modification in _modifications)
            {
                modification.modification.ResetOn(this);
            }
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_split)
            {
                var color = _spriteRenderer.color;
                color.a = Mathf.Lerp(1, 0, _elapsed);
                _spriteRenderer.color = color;
                
                if (!parts[0].gameObject.activeSelf && !parts[1].gameObject.activeSelf)
                {
                    Pool.Return(this);
                }
                return;
            }

            if (Player == null) return;

            if (Vector2.Distance(Player.transform.position.xy(), transform.position.xy()) <= minSplitDistance)
            {
                var dir = (Player.transform.position - transform.position).normalized;
                if (Vector3.Angle(dir, transform.up) < 10)
                {
                    Split();
                }
                else if (_elapsed >= lifeTime)
                {
                    Pool.Return(this);
                    // Split();
                }
            }
        }

        private void Split()
        {
            _split = true;
            _elapsed = 0;
            // Destroy(_rigidbody);
            _rigidbody.simulated = false;
            _collider.enabled = false;
            
            parts[0].transform.SetParent(null, true);
            parts[1].transform.SetParent(null, true);
            
            parts[0].StartMissile(1);
            parts[1].StartMissile(-1);
        }

        private void FixedUpdate()
        {
            if (_split || Player == null) return;
            
            var dir = (Player.transform.position - transform.position).normalized;
            dir.x *= XMultiplier;
            dir.y *= YMultiplier;
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