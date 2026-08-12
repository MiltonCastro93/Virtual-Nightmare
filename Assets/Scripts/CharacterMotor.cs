using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{//Mover Fisicamente al Personaje
    [SerializeField] private float moveSpeed = 5f, runSpeed = 10f;
    private bool isRunning;

    [SerializeField] private float gravity = -20f;

    private CharacterController characterController;
    private float verticalVelocity;

    [Header("Parametros del Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchCenter = 0.5f;
    [SerializeField] private float crouchDownSpeed = 8f, crouchUpSpeed = 6f;
    [SerializeField] private LayerMask environmentMask;//Mascara de deteccion

    private bool isCrouching;
    private float standingHeight;//valores predeterminados del CharacterController
    private Vector3 standingCenter;

    private void Awake() {
        //Obtengo el CharacterController de ref y sus valores para el crouch
        characterController = GetComponent<CharacterController>();
        standingHeight = characterController.height;
        standingCenter = characterController.center;
    }

    private void Update()
    {
        UpdateCrouch();
    }

    public void Move(Vector2 input)
    {
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        Vector3 movement = new Vector3(input.x, 0f, input.y);

        movement = transform.TransformDirection(movement);
        movement *= currentSpeed;

        // Gravedad
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    public void SetCrouching(bool crouching)
    {
        isCrouching = crouching;
    }

    private void UpdateCrouch()
    {
        float targetHeight = isCrouching
            ? crouchHeight
            : standingHeight;

        float targetCenterY = isCrouching
            ? crouchCenter
            : standingCenter.y;

        float transitionSpeed = isCrouching
            ? crouchDownSpeed
            : crouchUpSpeed;

        characterController.height = Mathf.MoveTowards(
            characterController.height,
            targetHeight,
            transitionSpeed * Time.deltaTime
        );

        Vector3 center = characterController.center;

        center.y = Mathf.MoveTowards(
            center.y,
            targetCenterY,
            transitionSpeed * Time.deltaTime
        );

        characterController.center = center;
    }
    public bool CanStandUp()//verificar si se puede levantar; Usada en dos clases PlayerController && CrouchingState
    {
        Vector3 bottom = transform.position
            + standingCenter
            + Vector3.up * characterController.radius;

        Vector3 top = transform.position
            + standingCenter
            + Vector3.up * (standingHeight - characterController.radius);

        return !Physics.CheckCapsule(
            bottom,
            top,
            characterController.radius,
            environmentMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public void StartSlide()
    {

    }

    public void StopSlider()
    {

    }

}

/*
 velocidad
aceleración
gravedad
salto
movimiento
agacharse
pendientes
colisiones
CharacterController
 */