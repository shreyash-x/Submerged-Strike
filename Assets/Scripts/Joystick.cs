using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float X { get; private set; } = 0;
    public float Y { get; private set; } = 0;

    public float DeltaX { get; private set; } = 0;
    public float DeltaY { get; private set; } = 0;

    public bool IsTouching => _using;

    [SerializeField] private float radius = 500; // radius in pixels
    
    private bool _using = false;
    private Vector2 _defPos;

    private void Awake()
    {
        _defPos = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _using = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _using = false;
        transform.position = _defPos;
    }
    
    private void Update()
    {
        if (!_using) return;
        // transform.position = eventData.position;
        
        var dir = (Input.mousePosition.xy() - _defPos);
        dir = Vector2.ClampMagnitude(dir, radius);
        transform.position = _defPos + dir;
    
        var px = X;
        var py = Y;
        X = dir.x / radius;
        Y = dir.y / radius;
    
        DeltaX = X - px;
        DeltaY = Y - py;
    }
}
