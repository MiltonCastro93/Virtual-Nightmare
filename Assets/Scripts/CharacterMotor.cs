using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{//Mover Fisicamente al Personaje
    [SerializeField] private float moveSpeed = 5f, runSpeed = 10f;
    private bool isRunning;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpForce = 8f;
    private CharacterController characterController;
    private float verticalVelocity;
    public float VerticalVelocity => verticalVelocity;

    [Header("Parametros del Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchCenter = 0.5f;
    [SerializeField] private float crouchDownSpeed = 8f, crouchUpSpeed = 6f;
    [SerializeField] private LayerMask environmentMask;//Mascara de deteccion

    [Header("Slide")]
    [SerializeField] private float slideSpeed = 14f;
    [SerializeField] private float slideDeceleration = 5f;
    private Vector3 slideDirection;
    private bool isSliding;
    private float currentSlideSpeed;
    private bool isCrouching;
    private float standingHeight;//valores predeterminados del CharacterController
    private Vector3 standingCenter;
    //Asi las clases pueden consultar el estado del Cc sin acceder directamente al componente
    public bool IsGrounded => characterController.isGrounded;

    [Header("Vault")]
    [SerializeField] private float vaultCheckDistance = 1.2f;
    [SerializeField] private float vaultMinHeight = 0.4f;//0.4 m < obstáculo válido > 1.2 m
    [SerializeField] private float vaultMaxHeight = 1.2f;
    [SerializeField] private float vaultLandingDistance = 1.2f;
    [Header("Vault Movement")]
    [SerializeField] private float vaultDuration = 0.5f;
    [SerializeField] private float vaultHeight = 0.8f;
    [SerializeField] private LayerMask vaultMask;
    private bool isVaulting;
    private float vaultProgress;
    private Vector3 vaultStartPosition;
    private Vector3 vaultEndPosition;

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
        UpdateVault();
    }

    public void Move(Vector2 input)
    {
        Vector3 movement;

        if (isSliding)
        {
            //para deslizar siempre hacia adelante
            movement = slideDirection * currentSlideSpeed;
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

    public void SetCrouching(bool crouching)// Llamado desde 2 clases, SlidingState; CrouchingState
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

    public void StartSlide()//Llamado desde la Clase SlidingState
    {
        if (isSliding) return;//incapacidad para no volver a inpulsarse

        isSliding = true;
        currentSlideSpeed = slideSpeed;
        slideDirection = transform.forward;
    }

    public void StopSlide()//Llamado desde la Clase SlidingState
    {
        isSliding = false;
        currentSlideSpeed = 0f;
    }

    private void UpdateSlide()//Desde el Update
    {
        if (!isSliding)
            return;

        currentSlideSpeed = Mathf.MoveTowards(
            currentSlideSpeed,
            0f,
            slideDeceleration * Time.deltaTime
        );
    }

    public bool IsSlideFinished()//Desde el PlayerController
    {
        return currentSlideSpeed <= 0.01f;
    }

    public void Jump()
    {
        if (!characterController.isGrounded) return;

        verticalVelocity = jumpForce;
    }

    public bool CanVault()//Parte analiza si puedo hacer Vault
    {
        Vector3 origin = transform.position + Vector3.up * vaultMaxHeight;

        if (!Physics.Raycast( origin, transform.forward, out RaycastHit obstacleHit,
            vaultCheckDistance, vaultMask, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        float obstacleHeight = obstacleHit.point.y - transform.position.y;

        if (obstacleHeight < vaultMinHeight || obstacleHeight > vaultMaxHeight)
        {
            return false;
        }

        Vector3 landingOrigin = obstacleHit.point +
            transform.forward * vaultLandingDistance + Vector3.up * 0.5f;

        if (!Physics.Raycast( landingOrigin, Vector3.down, out RaycastHit landingHit,
            1.5f, vaultMask, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;
    }

    public void StartVault()//Iniciar Sobre Salto
    {
        isVaulting = true;
        vaultProgress = 0f;

        vaultStartPosition = transform.position;

        vaultEndPosition = transform.position +
            transform.forward * vaultLandingDistance;
    }
    
    private void UpdateVault()//Loop para iniciar el sobre salto
    {
        if (!isVaulting)
            return;
        vaultProgress += Time.deltaTime / vaultDuration;

        float t = Mathf.Clamp01(vaultProgress);

        Vector3 targetPosition = Vector3.Lerp( vaultStartPosition, vaultEndPosition, t );
        targetPosition.y += Mathf.Sin(t * Mathf.PI) * vaultHeight;
        Vector3 movement = targetPosition - transform.position;

        characterController.Move(movement);

        if (t >= 1f)
        {
            isVaulting = false;
        }
    }

    public bool IsVaultFinished()
    {
        return !isVaulting;
    }

    public void StopVault()
    {
        isVaulting = false;
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