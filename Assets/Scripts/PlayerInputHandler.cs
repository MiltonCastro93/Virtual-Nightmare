using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{//Responsable en Detectar el Input del Jugador
    public event Action OnInteract;
    public event Action OnLeanLeft;
    public event Action OnLeanRight;
    public event Action OnLeanReset;
    public event Action OnRunStarted;
    public event Action OnRunCanceled;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    //----------------Player---------------------
    public void Move(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }

    public void Runner(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnRunStarted?.Invoke();
        }

        if (context.canceled)
        {
            OnRunCanceled?.Invoke();
        }
    }

    //----------------Camara----------------------
    public void LookCam(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void LeanLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnLeanLeft?.Invoke();
        }

        if (context.canceled)
        {
            OnLeanReset?.Invoke();
        }
    }

    public void LeanRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnLeanRight?.Invoke();
        }

        if (context.canceled)
        {
            OnLeanReset?.Invoke();
        }
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