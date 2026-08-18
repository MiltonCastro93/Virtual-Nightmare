using NUnit.Framework.Internal;
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

    [SerializeField] private float minVaultHeight = 0.45f;//Altura minima para hacer Vault
    [SerializeField] private float maxVaultHeight = 0.9f;//Altura Maxima para hacer Vault
    [SerializeField] private float maxClimbHeight = 1.8f;//Altura Maxima para hacer Climb
    [SerializeField] private float climbHeight = 2f;

    [Header("Vault")]
    [SerializeField] private float vaultDuration = 0.45f;
    [SerializeField] private float vaultForwardDistance = 1.2f;
    [SerializeField] private float vaultArcHeight = 0.8f;

    private bool isVaulting;
    private float vaultTimer;

    private Vector3 vaultStartPosition;
    private Vector3 vaultTargetPosition;

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

    public ObstacleData DetectObstacle()
    {
        Vector3 bottom = transform.position + standingCenter + Vector3.up * obstacleRadius;
        Vector3 top = transform.position + standingCenter + Vector3.up * (standingHeight - obstacleRadius);
        Vector3 direccionAdelante = transform.forward;

        if (Physics.CapsuleCast(bottom, top, obstacleRadius, direccionAdelante, out RaycastHit hitInfo, obstacleCheckDistance, environmentMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 topOrigin = hitInfo.point + Vector3.up * climbHeight + direccionAdelante * 0.05f;

            if (Physics.Raycast(topOrigin, Vector3.down, out RaycastHit topHit, Mathf.Infinity, environmentMask, QueryTriggerInteraction.Ignore))//1° Ray vertical
            {
                Debug.DrawRay(topOrigin, Vector3.down * climbHeight, Color.blue, 1f);

                float obstacleHeight = topHit.point.y - transform.position.y;

                //Lanzo otro Raycast para analizar la profundida y saber si es un objeto o ventana
                float profundidadCheck = 0.8f;
                Vector3 newTopOrigin = topOrigin + direccionAdelante * profundidadCheck;

                if(Physics.Raycast(newTopOrigin, Vector3.down, out RaycastHit newTopHit, Mathf.Infinity, environmentMask, QueryTriggerInteraction.Ignore))//2° Ray Vertical para verificar profundidad
                {
                    Debug.DrawRay(newTopOrigin, Vector3.down * climbHeight, Color.magenta, 1f);

                    float diferenciaDeAltura = Mathf.Abs(topHit.point.y - newTopHit.point.y);
                    float margenTolerancia = 0.1f; //10 centimetros

                    bool esSuperficiePlana = diferenciaDeAltura <= margenTolerancia; //False = ventana; True = Objeto

                    if (esSuperficiePlana)
                    {
                        //Nueva posicion de la capsula, necesito la posicion del inpacto y etc
                        Vector3 bArriba = topHit.point + Vector3.up * obstacleRadius;
                        Vector3 tArriba = topHit.point + Vector3.up * (standingHeight - obstacleRadius);

                        float espacioCheckAdelante = 0.5f; // Qué tan adelante queremos comprobar si cabe el cuerpo

                        //CapsulaCast para verificar si tengo lugar en la posicion
                        if (!Physics.CapsuleCast(bArriba, tArriba,obstacleRadius,direccionAdelante,out RaycastHit hitEspacio, espacioCheckAdelante, environmentMask, QueryTriggerInteraction.Ignore))
                        {
                            //Si entra en este if, el jugador cabe perfectamente
                            if (obstacleHeight >= minVaultHeight && obstacleHeight <= maxVaultHeight)
                            {
                                //Hacer Vault arriba de escritorio
                                return new ObstacleData(ObstacleType.Vault, hitEspacio.point);
                            }
                            else if (obstacleHeight > maxVaultHeight && obstacleHeight <= maxClimbHeight)
                            {
                                //Hacer Climb
                                return new ObstacleData(ObstacleType.Climb, hitEspacio.point);
                            }
                            //un else para hacer el Mantle
                        }
                        else
                        {
                            //Debug.Log("Espacio Obstruido con algo arriba");
                        }
                    }
                    else
                    {
                        //Debug.Log("Debo hacer Vaulting porque es una ventana/escritorio corto");
                        return new ObstacleData(ObstacleType.Vault, newTopHit.point);
                    }

                }
            }
        }

        return new ObstacleData(ObstacleType.None, Vector3.zero);
    }

    public void StartVault(Vector3 obstacleTop)
    {
        isVaulting = true;
        vaultTimer = 0f;

        vaultStartPosition = transform.position;

        vaultTargetPosition = obstacleTop + transform.forward * vaultForwardDistance;
    }
    private void UpdateVault()
    {
        if (!isVaulting)
            return;

        vaultTimer += Time.deltaTime;

        float t = vaultTimer / vaultDuration;
        t = Mathf.Clamp01(t);

        Vector3 position = Vector3.Lerp(
            vaultStartPosition,
            vaultTargetPosition,
            t
        );

        // Arco del vault
        position.y += Mathf.Sin(t * Mathf.PI) * vaultArcHeight;

        transform.position = position;

        if (t >= 1f)
        {
            isVaulting = false;
        }

    }

    public bool IsVaultFinished() { return !isVaulting; }

    private void OnDrawGizmos()
    {
        // Asegúrate de calcular las variables igual que en tu método de física
        // (Puedes hacer estas variables globales o propiedades para no repetir código)
        float obstacleRadius = 0.5f; // Usa tu variable real
        float standingHeight = 2.0f; // Usa tu variable real
        Vector3 standingCenter = transform.position; // Usa tu variable real
        float obstacleCheckDistance = 1.0f; // Usa tu variable real

        // El punto de abajo resta la mitad de la altura y suma el radio
        Vector3 bottom = transform.position + Vector3.up * (-(standingHeight / 2f) + obstacleRadius);

        // El punto de arriba suma la mitad de la altura y resta el radio
        Vector3 top = transform.position + Vector3.up * ((standingHeight / 2f) - obstacleRadius);
        Vector3 direccion = transform.forward;

        // --- 1. DIBUJAR LA CÁPSULA EN SU POSICIÓN INICIAL ---
        Gizmos.color = Color.yellow;
        // Esferas de los extremos
        Gizmos.DrawWireSphere(bottom, obstacleRadius);
        Gizmos.DrawWireSphere(top, obstacleRadius);
        // Líneas laterales que unen las esferas
        DibujarLineasCapsula(bottom, top, obstacleRadius);

        // --- 2. DIBUJAR EL RECORRIDO DEL BARRIDO (SWEEP) ---
        Gizmos.color = Color.cyan;
        Vector3 desplazamiento = direccion * obstacleCheckDistance;

        // Líneas que muestran hacia dónde se mueve la cápsula
        Gizmos.DrawLine(bottom, bottom + desplazamiento);
        Gizmos.DrawLine(top, top + desplazamiento);

        // Cápsula de destino (donde termina el cast)
        Gizmos.DrawWireSphere(bottom + desplazamiento, obstacleRadius);
        Gizmos.DrawWireSphere(top + desplazamiento, obstacleRadius);
        DibujarLineasCapsula(bottom + desplazamiento, top + desplazamiento, obstacleRadius);
    }

    // Función auxiliar para conectar las esferas de la cápsula con líneas
    private void DibujarLineasCapsula(Vector3 b, Vector3 t, float r)
    {
        Gizmos.DrawLine(b + Vector3.left * r, t + Vector3.left * r);
        Gizmos.DrawLine(b + Vector3.right * r, t + Vector3.right * r);
        Gizmos.DrawLine(b + Vector3.forward * r, t + Vector3.forward * r);
        Gizmos.DrawLine(b + Vector3.back * r, t + Vector3.back * r);
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