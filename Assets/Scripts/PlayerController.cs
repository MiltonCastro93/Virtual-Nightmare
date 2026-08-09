using System;
using UnityEngine;

public class PlayerController : CharacterHuman
{//Es el Coordinador, "El Jugador quiere hacer X. ¿Que sistema se Encarga de eso?"
    //es el Orquestador. "El Jugador quiere moverse; Motor, encargate" UwU

    private PlayerInputHandler inputHandler;
    private CharacterMotor motor;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CharacterInteractor interactor;

    protected override void Awake()
    {
        base.Awake();

        inputHandler = GetComponent<PlayerInputHandler>();
        motor = GetComponent<CharacterMotor>();

        //stateMachine.ChangeState(new IdleState());
    }

    private void OnEnable()
    {
        inputHandler.OnInteract += HandleInteract;//PlayerInputHandler <--- PlayerController
        inputHandler.OnRunStarted += HandleRunStarted;
        inputHandler.OnRunCanceled += HandleRunCanceled;

        inputHandler.OnLeanLeft += HandleLeanLeft;
        inputHandler.OnLeanRight += HandleLeanRight;
        inputHandler.OnLeanReset += HandleLeanReset;
    }

    private void OnDisable()
    {
        inputHandler.OnInteract -= HandleInteract;//PlayerInputHandler <--- PlayerController
        inputHandler.OnRunStarted -= HandleRunStarted;
        inputHandler.OnRunCanceled -= HandleRunCanceled;

        inputHandler.OnLeanLeft -= HandleLeanLeft;
        inputHandler.OnLeanRight -= HandleLeanRight;
        inputHandler.OnLeanReset -= HandleLeanReset;
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();
        HandleCamera();
    }

    private void HandleMovement()
    {//PlayerInputHandler --> PlayerController --> CharacterMotor
        motor.Move(inputHandler.MoveInput);
    }

    private void HandleCamera()
    {//PlayerInputHandler --> PlayerController --> CameraController
        cameraController.Look(inputHandler.LookInput);
    }
    //------------------------------
    private void HandleInteract()
    {//PlayerInputHandler --> PlayerController --> CharacterInteractor
        interactor.Interact();
    }

    private void HandleRunStarted()
    {
        motor.SetRunning(true);
    }

    private void HandleRunCanceled()
    {
        motor.SetRunning(false);
    }

    private void HandleLeanLeft()
    {
        cameraController.LeanLeft();
    }

    private void HandleLeanRight()
    {
        cameraController.LeanRight();
    }

    private void HandleLeanReset()
    {
        cameraController.ResetLean();
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