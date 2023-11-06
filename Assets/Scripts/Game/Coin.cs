using UnityEngine;

namespace Game
{
    public class Coin : MonoBehaviour
    {
        internal Pool<Coin> Pool;
        
        [SerializeField] private float lifeTime;
        [SerializeField] private float dieAnimationTime;
        [SerializeField] private GameObject offScreenIcon;
        [SerializeField] private float screenBorder = 0.1f;

        internal GameObject Player;
        
        private float _elapsed;
        private Vector3 _defaultScale;

        private GameObject _icon;

        private SpriteRenderer _renderer;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            _defaultScale = transform.localScale;
            _renderer = GetComponent<SpriteRenderer>();
            
            _icon = Instantiate(offScreenIcon);
            _icon.SetActive(false);
        }

        private void OnEnable()
        {
            transform.localScale = _defaultScale;
            _elapsed = 0;
        }

        private void OnDisable()
        {
            if (_icon != null)
            {
                _icon.SetActive(false);
            }
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= lifeTime)
            {
                var elapsed = _elapsed - lifeTime;
                var t =  elapsed / dieAnimationTime;
                transform.localScale = _defaultScale * Mathf.Lerp(1, 0, t);
                if(elapsed >= dieAnimationTime) Pool.Return(this);
            }
        }

        private void LateUpdate()
        {
            PositionIcon();
        }

        private void PositionIcon()
        {
            if (!_renderer.isVisible && Player != null)
            {
                if(!_icon.activeSelf) _icon.SetActive(true);

                var position = transform.position;
                _icon.transform.position = Utility.WorldPosToBorder(position, screenBorder, _camera);
                _icon.transform.up = (position - Player.transform.position).normalized;
            }
            else
            {
                if(_icon.activeSelf) _icon.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.IsPartOfLayer("Player"))
            {
                Pool.Return(this);
            }
        }
    }
}