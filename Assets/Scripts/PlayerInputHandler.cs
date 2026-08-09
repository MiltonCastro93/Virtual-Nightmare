using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{//Responsable en Detectar el Input del Jugador
    public event Action OnInteract;

    public Vector2 MoveInput { get; private set; }

    public void Move(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        Debug.Log($"Move Input: {MoveInput}");
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interaccion");

        OnInteract?.Invoke();
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