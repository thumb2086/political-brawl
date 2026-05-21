using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Joystick movementJoystick;
    public Joystick attackJoystick;
    public bool superPressed;
    public bool gadget1Pressed;
    public bool gadget2Pressed;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector2 GetMoveDirection()
    {
        if (movementJoystick != null && movementJoystick.isActive)
            return movementJoystick.GetDirection();

        if (Application.isEditor || SystemInfo.deviceType == DeviceType.Desktop)
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        return Vector2.zero;
    }

    public Vector2 GetAimDirection()
    {
        if (attackJoystick != null && attackJoystick.isActive)
            return attackJoystick.GetDirection();

        if (Application.isEditor || SystemInfo.deviceType == DeviceType.Desktop)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return (mousePos - GetPlayerPosition()).normalized;
        }

        return Vector2.zero;
    }

    public bool IsAttacking()
    {
        if (attackJoystick != null && attackJoystick.isActive)
            return true;

        if (Application.isEditor || SystemInfo.deviceType == DeviceType.Desktop)
        {
            return Input.GetMouseButton(0);
        }

        return false;
    }

    public bool IsSuperPressed()
    {
        if (superPressed)
        {
            superPressed = false;
            return true;
        }

        if (Application.isEditor || SystemInfo.deviceType == DeviceType.Desktop)
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        return false;
    }

    public bool IsGadget1Pressed()
    {
        if (gadget1Pressed)
        {
            gadget1Pressed = false;
            return true;
        }

        if (Application.isEditor || SystemInfo.deviceType == DeviceType.Desktop)
        {
            return Input.GetKeyDown(KeyCode.Q);
        }

        return false;
    }

    public bool IsGadget2Pressed()
    {
        if (gadget2Pressed)
        {
            gadget2Pressed = false;
            return true;
        }

        if (Application.isEditor || SystemInfo.deviceType == DeviceType.Desktop)
        {
            return Input.GetKeyDown(KeyCode.E);
        }

        return false;
    }

    Vector3 GetPlayerPosition()
    {
        if (Camera.main == null) return Vector3.zero;
        if (GameManager.Instance != null && GameManager.Instance.players != null && GameManager.Instance.players.Length > 0)
        {
            if (GameManager.Instance.players[0] != null)
                return GameManager.Instance.players[0].transform.position;
        }
        return Vector3.zero;
    }
}
