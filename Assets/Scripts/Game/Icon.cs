using Game.Enemy;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(EnemyBase))]
    public class Icon : MonoBehaviour
    {
        [SerializeField] private GameObject iconPrefab;
        [SerializeField] private float screenBorder;


        private EnemyBase _enemyComponent;

        private Renderer _renderer;
        private GameObject _icon;
        private Camera _camera;
        
        private void Awake()
        {
            _enemyComponent = GetComponent<EnemyBase>();
            _renderer = GetComponent<Renderer>();
            _camera = Camera.main;
            
            _icon = Instantiate(iconPrefab);
            _icon.SetActive(false);
        }

        private void OnDisable()
        {
            if(_icon != null) _icon.SetActive(false);
        }

        private bool Display() => _enemyComponent.IsActive() && !_renderer.isVisible && _enemyComponent.Player != null;
        
        private void LateUpdate()
        {
            if (Display())
            {
                if(!_icon.activeSelf) _icon.SetActive(true);
                
                _icon.transform.position = Utility.WorldPosToBorder(transform.position, screenBorder, _camera);
                _icon.transform.up = (_enemyComponent.Player.transform.position - _icon.transform.position).normalized;
            } else if(_icon.activeSelf) _icon.SetActive(false);
        }
    }
}