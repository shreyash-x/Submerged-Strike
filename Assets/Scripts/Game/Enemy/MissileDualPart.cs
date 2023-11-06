using System.Collections.Generic;
using Game.ModificationSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    [RequireComponent(typeof(Icon))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class MissileDualPart : EnemyBase, IAllowXModification, IAllowYModification, IAllowAttackFriendlyModification, IAllowSelfDestructModification
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private float zeroSpeedTime;
        [SerializeField] private float followAfterTime;
        [SerializeField] private float dieAnimationTime;
        [SerializeField] private float deflectionAngle;
        [SerializeField] private float noModificationProbability;
        [SerializeField] private RigidbodyController controller;
        [SerializeField] private TrailRenderer trail;

        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        
        private bool _started = false;
        private bool _following = false;
        private bool _dead = false;
        private float _elapsed = 0;
        private Vector3 _dir;

        private float _defaultSpeed;
        private Vector3 _defaultScale;

        public float XMultiplier { get; set; } = 1;
        public float YMultiplier { get; set; } = 1;

        public GameObject Target
        {
            get => _target;
            set
            {
                _isTargetNull = value == null;
                _target = value;
            }
        }

        public override bool IsActive() => _started && !_dead && gameObject.activeSelf;

        private bool _isTargetNull = true;


        private readonly List<(float probablity, IModification<MissileDualPart> modification)> _modifications = new List<(float probablity, IModification<MissileDualPart>)>()
        {
            (0.3f, new InvertXModification()),
            (0.3f, new InvertYModification()),
            (0.3f, new AttackFriendlyModification()),
            (0.1f, new SelfDestructModification())
        };

        private GameObject _target;


        // direction: 1 up -1 down
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.simulated = false;
            
            _collider = GetComponent<Collider2D>();
            _collider.enabled = false;
            _defaultSpeed = controller.speed;
        }

        public void SelfDestruct()
        {
            var go = Instantiate(explosion);
            go.ExplodeAt(transform.position);
            Pool.Return(this);
        }

        public void StartMissile(int direction)
        {
            _started = true;
            _collider.enabled = true;
            _dir = Quaternion.AngleAxis(deflectionAngle * direction, Vector3.forward) * transform.up;

            _rigidbody.simulated = true;
            controller.rigidbody = _rigidbody;
            // controller.speed = 0;
            
            trail.emitting = true;
            _defaultScale = transform.localScale;
            controller.Init(transform.up.xy());

            Target = Player;
        }

        private void OnEnable()
        {
            transform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            _started = _following = _dead = false;
            showExplosion = true;
            _elapsed = 0;
            
            trail.Clear();
            trail.emitting = false;
            trail.widthMultiplier = 1;
            
            foreach (var modification in _modifications)
            {
                modification.modification.ResetOn(this);
            }
        }

        private void Update()
        {
            if (!_started) return;
            
            _elapsed += Time.deltaTime;
            if (_elapsed >= lifeTime)
            {
                var elapsed = _elapsed - lifeTime;
                var t =  elapsed / dieAnimationTime;
                transform.localScale = _defaultScale * Mathf.Lerp(1, 0, t);
                trail.widthMultiplier = Mathf.Lerp(1, 0, t);
                
                _dead = true;
                if (elapsed >= dieAnimationTime) gameObject.SetActive(false);
            } else if (_elapsed >= followAfterTime)
            {
                _following = true;
            } 
        }

        private void FixedUpdate()
        {
            //if (!_started || Target == null) return;
            if (!_started || _isTargetNull) return;
            
            if (_dead)
            {
                _rigidbody.drag = 0.5f;
                return;
            }
            
            _rigidbody.drag = 10;
            if (_following && Target != null)
            {
                var dir = (Target.transform.position - transform.position).normalized;
                dir.x *= XMultiplier;
                dir.y *= YMultiplier;
                controller.Update(dir);
            }
            else
            {
                controller.Update(_dir);
            }
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
                gameObject.SetActive(false);
            } else if(other.IsPartOfLayer("Player"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}