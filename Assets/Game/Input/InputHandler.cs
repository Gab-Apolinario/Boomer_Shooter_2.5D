using UnityEngine;

public class InputHandler
{
    public InputSystem_Actions inputActions;

    public InputHandler()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    public Vector2 MoveInput
    {
        get { return inputActions.Player.Move.ReadValue<Vector2>(); }
    }

    public Vector2 LookInput
    {
        get { return inputActions.Player.Look.ReadValue<Vector2>(); }
    }

    public bool JumpInput
    {
        get { return inputActions.Player.Jump.ReadValue<float>() > 0.5f; }
    }

    public bool IsSprinting
    {
        get { return inputActions.Player.Sprint.ReadValue<float>() > 0.5f; }
    }
    
    public bool IsShooting
    {
        get { return inputActions.Player.Shoot.ReadValue<float>() > 0.5f; }
    }

    public bool IsReloading
    {
        get { return inputActions.Player.Reload.ReadValue<float>() > 0.5f; }
    }

    public bool IsMeleeAttacking
    {
        get { return inputActions.Player.MeleeAtack.ReadValue<float>() > 0.5f; }
    }

    public bool IsDashing
    {
        get { return inputActions.Player.Dash.ReadValue<float>() > 0.5f; }
    }
}