using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{//Mover Fisicamente al Personaje
    [SerializeField] private float moveSpeed = 5f, runSpeed = 10f;
    private bool isRunning;

    [SerializeField] private float gravity = -20f;

    private CharacterController characterController;
    private float verticalVelocity;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
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
        Debug.Log("Esta Corriendo?: " + running);
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