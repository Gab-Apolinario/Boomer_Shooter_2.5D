using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler
{
    public InputSystem_Actions inputActions;

    public static InputHandler Instance { get; private set; }

    public InputHandler()
    {
        Instance = this;
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private bool isGamepad = false;

    public bool IsGamepad => isGamepad;

    public void UpdateActiveDevice()
    {
        if (Gamepad.current != null && 
            (Gamepad.current.leftStick.ReadValue().magnitude > 0.1f ||
            Gamepad.current.rightStick.ReadValue().magnitude > 0.1f ||
            Gamepad.current.rightTrigger.ReadValue() > 0.1f))
            {
                isGamepad = true;
            }
        else if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame || Mouse.current != null && Mouse.current.delta.ReadValue().magnitude > 0.1f)
            {
                isGamepad = false;
            }
    }

    public Vector2 MoveInput
    {
        get { return inputActions.Player.Move.ReadValue<Vector2>(); }
    }

    public Vector2 LookInput
    {
        get { return inputActions.Player.Look.ReadValue<Vector2>(); }
    }

    public bool IsShooting
    {
        get { return inputActions.Player.Shoot.ReadValue<float>() > 0.5f; }
    }

    public bool IsDashing
    {
        get { return inputActions.Player.Dash.ReadValue<float>() > 0.5f; }
    }
}