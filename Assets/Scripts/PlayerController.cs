using System;
using UnityEngine;

public class PlayerController : CharacterHuman
{//Es el Coordinador, "El Jugador quiere hacer X. ¿Que sistema se Encarga de eso?"
    //es el Orquestador. "El Jugador quiere moverse; Motor, encargate" UwU

    private PlayerInputHandler inputHandler;
    private CharacterMotor motor;

    protected override void Awake()
    {
        base.Awake();

        inputHandler = GetComponent<PlayerInputHandler>();
        motor = GetComponent<CharacterMotor>();

        //stateMachine.ChangeState(new IdleState());
    }

    private void OnEnable()
    {
        inputHandler.OnInteract += HandleInteract;
    }

    private void OnDisable()
    {
        inputHandler.OnInteract -= HandleInteract;
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();
    }

    private void HandleMovement()
    {
        motor.Move(inputHandler.MoveInput);
    }

    private void HandleInteract()
    {
        Debug.Log("El jugador quiere interactuar");
    }
}

/* pasa la tarea mirando quien la hace
 PlayerInputHandler
       │
       ├── MoveInput
       ├── Sprint
       ├── Crouch
       └── Interact
       │
       ▼
PlayerController
       │
       ├── CharacterMotor
       ├── Stamina
       ├── CharacterInteractor
       └── ...
 //"El jugador interactuó con un escondite" por lo que avisara al characterstatemaquine
 */