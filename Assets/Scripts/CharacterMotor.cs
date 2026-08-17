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

    [Header("Obstacle Detection")]
    [SerializeField] private float obstacleCheckDistance = 1f;
    [SerializeField] private float obstacleRadius = 0.35f;

    [SerializeField] private float minVaultHeight = 0.45f;
    [SerializeField] private float maxVaultHeight = 0.9f;
    [SerializeField] private float maxClimbHeight = 1.8f;
    [SerializeField] private float climbHeight = 2f;

    [SerializeField] private float detectionBottomHeight = 0.1f;
    [SerializeField] private float detectionTopHeight = 1.7f;

    [SerializeField] private float landingCheckDistance = 1f;

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

    public ObstacleType DetectObstacle()
    {
        Vector3 castBottom = transform.position + Vector3.up * detectionBottomHeight;
        Vector3 castTop = transform.position + Vector3.up * detectionTopHeight;
        Vector3 direction = transform.forward;

        Debug.DrawLine( castBottom, castBottom + direction * obstacleCheckDistance,
            Color.red, 1f );

        //Compruebo si hay algun Obstaculo con una Capsula
        if (!Physics.CapsuleCast( castBottom, castTop, obstacleRadius,
            direction, out RaycastHit hit, obstacleCheckDistance, environmentMask,
            QueryTriggerInteraction.Ignore))
        {
            return ObstacleType.None;
        }

        // Encontramos algo delante. Ahora buscamos la superficie superior.
        Vector3 topOrigin = hit.point + Vector3.up * climbHeight + direction * 0.05f;

        Debug.DrawRay( topOrigin, Vector3.down * climbHeight, Color.blue, 1f );

        if (!Physics.Raycast( topOrigin, Vector3.down, out RaycastHit topHit,
            climbHeight, environmentMask, QueryTriggerInteraction.Ignore))
        {
            return ObstacleType.None;
        }

        float obstacleHeight = topHit.point.y - transform.position.y;

        Debug.Log("Altura obstáculo: " + obstacleHeight);

        if (obstacleHeight <= 0f)
        {
            return ObstacleType.None;
        }

        if (obstacleHeight >= minVaultHeight && obstacleHeight <= maxVaultHeight)
        {
            return ObstacleType.Vault;
        }

        if (obstacleHeight > maxVaultHeight && obstacleHeight <= maxClimbHeight)
        {
            return ObstacleType.Climb;
        }

        return ObstacleType.None;
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