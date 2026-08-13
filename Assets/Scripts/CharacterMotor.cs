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

    [Header("Slide")]
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideDeceleration = 8f;

    private bool isSliding;
    private float currentSlideSpeed;
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
        UpdateSlide();
    }

    public void Move(Vector2 input)
    {
        Vector3 movement;

        if (isSliding)
        {
            //para deslizar siempre hacia adelante
            movement.x = 0f;
            movement = transform.forward * currentSlideSpeed;
        }
        else
        {
            //para moverse libremente
            float currentSpeed = isRunning ? runSpeed : moveSpeed;

            movement = new Vector3(input.x, 0f, input.y);
            movement = transform.TransformDirection(movement);
            movement *= currentSpeed;
        }

        // Gravedad
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        movement.y = verticalVelocity;

        characterController.Move(
            movement * Time.deltaTime
        );
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
    {//actualizar la capsula collider del CharacterController
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
        isSliding = true;
        currentSlideSpeed = slideSpeed;
    }

    public void StopSlide()
    {
        isSliding = false;
        currentSlideSpeed = 0f;
    }

    private void UpdateSlide()
    {
        if (!isSliding)
            return;

        currentSlideSpeed = Mathf.MoveTowards(
            currentSlideSpeed,
            0f,
            slideDeceleration * Time.deltaTime
        );
    }

    public bool IsSlideFinished()
    {
        return currentSlideSpeed <= 0.01f;
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