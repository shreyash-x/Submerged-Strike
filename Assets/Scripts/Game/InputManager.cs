using UnityEngine;

namespace Game
{
    public enum InputMode
    {
        Keyboard,
        Joystick
    }
    
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputMode inputMode = InputMode.Joystick;
        [SerializeField] private Joystick[] joystick;
        [SerializeField] private GameObject[] joystickUI;
        [SerializeField] private float keyboardSensitivity = 5;

        //private Vector2 _currentDir = Vector2.up;
        private float _currentAngle = 0;
        
        public void SetInputMode(InputMode mode)
        {
            inputMode = mode;
            if (mode == InputMode.Keyboard)
            {
                foreach (var o in joystickUI) o.SetActive(false);
            }
            else
            {
                foreach (var o in joystickUI) o.SetActive(true);
            }
        }
        
        public Vector2 GetMoveDirection()
        {
            switch (inputMode)
            {
                case InputMode.Joystick:
                    bool found = false;
                    Joystick joy = null;
                    foreach (var joystick in joystick)
                    {
                        if (joystick.isActiveAndEnabled)
                        {
                            joy = joystick;
                            found = true;
                            break;
                        }
                    }

                    // return Vector2.up;
                    // return found ? new Vector2(joy.X, joy.Y) : Vector2.up;
                    return new Vector2(joy.X, joy.Y);
                case InputMode.Keyboard:
                {
                     float angle = Mathf.LerpAngle(_currentAngle, _currentAngle + Input.GetAxis("Horizontal") * keyboardSensitivity * Time.deltaTime, 1);
                     Vector3 dir = Quaternion.AngleAxis(angle, -Vector3.forward) * Vector3.up;
                     _currentAngle = angle;
                    return dir;
                }
                default:
                    return Vector2.zero;
            }
        }
    }
}