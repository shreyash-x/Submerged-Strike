using UnityEngine;
using UnityEngine.Animations;

namespace Game.Enemy
{
    public class EnemyIndicator : MonoBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] private GameObject iconPrefab;
        
        internal GameObject Player;
        internal EnemySpawner EnemySpawner;

        private void Awake()
        {
        }

        private void Start()
        {
            var positionConstraint = gameObject.AddComponent<PositionConstraint>();
            positionConstraint.AddSource(new ConstraintSource()
            {
                sourceTransform = Player.transform,
                weight = 1
            });
            positionConstraint.constraintActive = true;
        }

        private void Update()
        {
            var array = EnemySpawner.Active;
            
            int count = array.Count;
            if(count == 0) return;

            for (int i = 0; i < count; i++)
            {
                var dir = (array[i].transform.position - transform.position).normalized;
            }
        }
    }
}