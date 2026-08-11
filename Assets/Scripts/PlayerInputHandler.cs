using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{//Responsable en Detectar el Input del Jugador
    // INPUT CONTINUO
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    // EVENTOS
    public event Action OnMoveStarted;//ayuda para el PlayerController se de cuenta cuando se empiece a mover
    public event Action OnMoveCanceled;
    public event Action OnInteract;
    public event Action OnRunStarted;
    public event Action OnRunCanceled;
    public event Action OnLeanLeft;
    public event Action OnLeanRight;
    public event Action OnLeanReset;
    public event Action OnLookBack;
    public event Action OnLookBackCanceled;
    public event Action OnCrouchStarted;
    public event Action OnCrouchCanceled;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += HandleMove;
        inputActions.Player.Move.canceled += HandleMove;

        inputActions.Player.Look.performed += HandleLook;
        inputActions.Player.Look.canceled += HandleLook;

        inputActions.Player.Interact.performed += HandleInteract;

        inputActions.Player.Sprint.started += HandleRunStarted;
        inputActions.Player.Sprint.canceled += HandleRunCanceled;

        inputActions.Player.LeanLeft.started += HandleLeanLeft;
        inputActions.Player.LeanLeft.canceled += HandleLeanReset;

        inputActions.Player.LeanRight.started += HandleLeanRight;
        inputActions.Player.LeanRight.canceled += HandleLeanReset;

        inputActions.Player.LookBack.started += HandleLookBackStarted;
        inputActions.Player.LookBack.canceled += HandleLookBackCanceled;

        inputActions.Player.Crouch.started += HandleCrouchStarted;
        inputActions.Player.Crouch.canceled += HandleCrouchCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= HandleMove;
        inputActions.Player.Move.canceled -= HandleMove;

        inputActions.Player.Look.performed -= HandleLook;
        inputActions.Player.Look.canceled -= HandleLook;

        inputActions.Player.Interact.performed -= HandleInteract;

        inputActions.Player.Sprint.started -= HandleRunStarted;
        inputActions.Player.Sprint.canceled -= HandleRunCanceled;

        inputActions.Player.LeanLeft.started -= HandleLeanLeft;
        inputActions.Player.LeanLeft.canceled -= HandleLeanReset;

        inputActions.Player.LeanRight.started -= HandleLeanRight;
        inputActions.Player.LeanRight.canceled -= HandleLeanReset;

        inputActions.Player.LookBack.started -= HandleLookBackStarted;
        inputActions.Player.LookBack.canceled -= HandleLookBackCanceled;

        inputActions.Player.Crouch.started -= HandleCrouchStarted;
        inputActions.Player.Crouch.canceled -= HandleCrouchCanceled;

        inputActions.Player.Disable();
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        if (context.performed)
        {
            OnMoveStarted?.Invoke();
        }
        else if (context.canceled)
        {
            OnMoveCanceled?.Invoke();
        }
    }

    private void HandleLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }

    private void HandleRunStarted(InputAction.CallbackContext context)
    {
        OnRunStarted?.Invoke();
    }

    private void HandleRunCanceled(InputAction.CallbackContext context)
    {
        OnRunCanceled?.Invoke();
    }

    private void HandleLeanLeft(InputAction.CallbackContext context)
    {
        OnLeanLeft?.Invoke();
    }

    private void HandleLeanRight(InputAction.CallbackContext context)
    {
        OnLeanRight?.Invoke();
    }

    private void HandleLeanReset(InputAction.CallbackContext context)
    {
        OnLeanReset?.Invoke();
    }

    private void HandleLookBackStarted(InputAction.CallbackContext context)
    {
        OnLookBack?.Invoke();
    }

    private void HandleLookBackCanceled(InputAction.CallbackContext context)
    {
        OnLookBackCanceled?.Invoke();
    }

    private void HandleCrouchStarted(InputAction.CallbackContext context)
    {
        OnCrouchStarted?.Invoke();
    }

    private void HandleCrouchCanceled(InputAction.CallbackContext context)
    {
        OnCrouchCanceled?.Invoke();
    }

}

/*Su trabajo es traducir:
 WASD       → MoveInput
Mouse      → LookInput
Shift      → Sprint
Ctrl       → Crouch
E          → Interact
F          → Flashlight
Tab        → Inventory
 
 */