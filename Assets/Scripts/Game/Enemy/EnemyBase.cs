using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected Explosion explosion;
        
        public GameObject Player { get; set; }
        public Pool<EnemyBase> Pool { get; set; }
        public IReadOnlyList<GameObject> Friendlies { get; set; }
        public Func<EnemyBase, Pool<EnemyBase>> GetPool;
        
        public bool showExplosion = true;

        public abstract bool IsActive();
    }
}