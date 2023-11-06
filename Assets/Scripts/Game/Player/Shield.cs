using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Game.Player
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Shield : MonoBehaviour
    {
        public float angle = 30;
        public float deltaAngle = 5; // angle of top vertex of each triangle
        public float ringWidth = 5; // ring width in world units
        public float ringStart = 10; // distance at which ring starts

        private PolygonCollider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private static readonly int Angle = Shader.PropertyToID("_Angle");

        internal GameObject Player;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<PolygonCollider2D>();
        
            _spriteRenderer.sharedMaterial.SetFloat(Angle, angle);

            var vertex = new List<Vector2>();
            // angle *= Mathf.Deg2Rad;
            // deltaAngle *= Mathf.Deg2Rad;
        
            var from = 90 + angle;
            var to = 90 - angle;
            var i = Mathf.Min(from, to);
            to = Mathf.Max(from, to);
            from = i;
        
            while (i <= to)
            {
                var dir = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), 0);
                var p1 = ringStart * dir;
                vertex.Add(p1);
                if (Mathf.Approximately(i, to)) break;
                i += deltaAngle;
                if (i >= to) i = to;
            }
        
            while (i >= from)
            {
                var dir = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), 0);
                var p1 = (ringStart + ringWidth) * dir;
                vertex.Add(p1);
                if (Mathf.Approximately(i, from)) break;
                i -= deltaAngle;
                if (i <= from) i = from;
            }
            _collider.SetPath(0, vertex.ToArray());
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
            
            var rotationConstraint = gameObject.AddComponent<RotationConstraint>();
            rotationConstraint.AddSource(new ConstraintSource()
            {
                sourceTransform = Player.transform,
                weight = 1
            });
            rotationConstraint.constraintActive = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((other.gameObject.layer & LayerMask.NameToLayer("CosmicRay")) == 0) return;
        
            var position = transform.position;
            var contactPoint = _collider.ClosestPoint(position);
        
            var normal = (contactPoint - position.xy()).normalized;
            var incident = other.transform.up;
            var reflected = Vector2.Reflect(incident, normal);
        
            // other.transform.position = contactPoint;
            other.transform.up = reflected;
        }
    }
}