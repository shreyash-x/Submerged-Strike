using System;
using UnityEngine;

[Serializable]
public class RigidbodyController
{
    public float speed = 100;
    public float smoothMoveRotation = 360;
    public float smoothLookRotation = 360;
    
    public bool IsRotating { get; private set; }
    
    [HideInInspector] public Rigidbody2D rigidbody;

    private Vector2 _moveDir = Vector2.up;
    private Vector2 _angularVelocity;

    private float _angle = 0;

    public void Init(Vector2 dir)
    {
        _moveDir = dir;
    }

    public void Update(Vector2 input)
    {
        _moveDir = Vector3.RotateTowards(_moveDir, input, smoothMoveRotation * Time.fixedDeltaTime * Mathf.Deg2Rad, 0f);
        
        // var angle = Mathf.Abs(Vector2.SignedAngle(_moveDir, input));
        // _moveDir = Vector2.SmoothDamp(_moveDir, input, ref _angularVelocity, angle / (smoothMoveRotation), smoothMoveRotation * Time.fixedDeltaTime, Time.fixedDeltaTime).normalized;
        
        rigidbody.AddForce(_moveDir * (speed * Time.fixedDeltaTime), ForceMode2D.Impulse);

        // rigidbody.drag = (1 - Time.timeScale) * 10000000;
        
        var dir = rigidbody.velocity.normalized;
        var angle = Vector2.SignedAngle(Vector2.up, dir);
        var rot = Mathf.MoveTowardsAngle(rigidbody.rotation, angle, smoothLookRotation * Time.fixedDeltaTime);
        var delta = Mathf.DeltaAngle(rigidbody.rotation, rot);
        IsRotating = !Mathf.Approximately(delta, 0);
        
        rigidbody.MoveRotation(rot);
    }
}