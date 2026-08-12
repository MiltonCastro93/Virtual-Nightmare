using System;
using UnityEngine;

public class PlayerController : CharacterHuman
{//Es el Coordinador, "El Jugador quiere hacer X. ¿Que sistema se Encarga de eso?"
    //es el Orquestador. "El Jugador quiere moverse; Motor, encargate" UwU

    private PlayerInputHandler inputHandler;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CharacterInteractor interactor;
    private bool isLeaning, isLookingBack, crouchHeld; //Bool Banderas para detectar si estoy inclinado, mirando mi nuca, Levantarse
    protected SlidingState slidingState;
    protected CrouchingState crouchingState;


    protected override void Awake()
    {
        base.Awake();
        slidingState = new SlidingState(motor);
        crouchingState = new CrouchingState(motor);

        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void OnEnable()
    {
        inputHandler.OnInteract += HandleInteract;//PlayerInputHandler <--- PlayerController

        inputHandler.OnMoveStarted += HandleMoveStarted;//ayuda a cambiar entre idle a walking
        inputHandler.OnMoveCanceled += HandleMoveCanceled;

        inputHandler.OnRunStarted += HandleRunStarted;
        inputHandler.OnRunCanceled += HandleRunCanceled;

        inputHandler.OnLookBack += HandleLookBack;
        inputHandler.OnLookBackCanceled += HandleLookBackCanceled;

        inputHandler.OnLeanLeft += HandleLeanLeft;
        inputHandler.OnLeanRight += HandleLeanRight;
        inputHandler.OnLeanReset += HandleLeanReset;

        inputHandler.OnCrouchStarted += HandleCrouchStarted;
        inputHandler.OnCrouchCanceled += HandleCrouchCanceled;
    }

    private void OnDisable()
    {
        inputHandler.OnInteract -= HandleInteract;//PlayerInputHandler <--- PlayerController

        inputHandler.OnMoveStarted -= HandleMoveStarted;
        inputHandler.OnMoveCanceled -= HandleMoveCanceled;

        inputHandler.OnRunStarted -= HandleRunStarted;
        inputHandler.OnRunCanceled -= HandleRunCanceled;

        inputHandler.OnLookBack -= HandleLookBack;
        inputHandler.OnLookBackCanceled -= HandleLookBackCanceled;

        inputHandler.OnLeanLeft -= HandleLeanLeft;
        inputHandler.OnLeanRight -= HandleLeanRight;
        inputHandler.OnLeanReset -= HandleLeanReset;

        inputHandler.OnCrouchStarted -= HandleCrouchStarted;
        inputHandler.OnCrouchCanceled -= HandleCrouchCanceled;
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();
        HandleCamera();
        HandleCrouchAutoStand();//Verifica en que estado volver

        Debug.Log("Estado Actual: " + stateMachine.currentState.ToString());
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

    private void HandleMoveStarted()
    {
        if (stateMachine.currentState == idleState)
        {
            stateMachine.ChangeState(walkingState);
        }
    }

    private void HandleMoveCanceled()
    {
        if (stateMachine.currentState == walkingState ||
            stateMachine.currentState == runningState)
        {
            stateMachine.ChangeState(idleState);
        }
    }

    private void HandleRunStarted()
    {
        // Cancelar Lean
        if (isLeaning)
        {
            isLeaning = false;
            cameraController.ResetLean();
        }

        // Cancelar mirar atrás
        if (isLookingBack)
        {
            isLookingBack = false;
            cameraController.ResetLookBack();
        }


        stateMachine.ChangeState(runningState);
    }

    private void HandleRunCanceled()
    {
        if(inputHandler.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(walkingState);
        }
        else
        {
            stateMachine.ChangeState(idleState);
        }

    }

    private void HandleLeanLeft()
    {
        //Si estoy corriendo no deja entrar en el evento
        if (stateMachine.currentState == runningState)
            return;

        isLeaning = true;

        cameraController.LeanLeft();
    }

    private void HandleLeanRight()
    {
        if (stateMachine.currentState == runningState)
            return;

        isLeaning = true;

        cameraController.LeanRight();
    }

    private void HandleLeanReset()
    {
        isLeaning = false;

        cameraController.ResetLean();
    }

    private void HandleLookBack()
    {
        if (stateMachine.currentState == runningState)
        {
            return;
        }

        isLookingBack = true;

        cameraController.LookBack();
    }

    private void HandleLookBackCanceled()
    {
        //Puede ocurrir que quiera cancelar 2 veces
        if (!isLookingBack)
            return;

        isLookingBack = false;

        cameraController.ResetLookBack();

    }

    private void HandleCrouchStarted()
    {
        crouchHeld = true;

        if (stateMachine.currentState == runningState)
        {
            stateMachine.ChangeState(slidingState);
            return;
        }

        if (stateMachine.currentState == crouchingState)
        {
            return;
        }

        if (stateMachine.currentState == idleState ||
            stateMachine.currentState == walkingState)
        {
            stateMachine.ChangeState(crouchingState);
        }
    }

    private void HandleCrouchCanceled()
    {
        crouchHeld = false;

        if (stateMachine.currentState != crouchingState)
            return;

        // No hay espacio para levantarse, Hace un chequeo en el CharacterMotor
        if (!motor.CanStandUp())
            return;

        if (inputHandler.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(walkingState);
        }
        else
        {
            stateMachine.ChangeState(idleState);
        }
    }

    private void HandleCrouchAutoStand()
    {
        if (stateMachine.currentState != crouchingState)
            return;

        if (crouchHeld)
            return;

        if (!crouchingState.CanExit())//Metodo Expuesto de la Clase CrouchingState
            return;

        if (inputHandler.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(walkingState);
        } else {
            stateMachine.ChangeState(idleState);
        }
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