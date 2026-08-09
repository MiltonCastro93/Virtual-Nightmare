using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{//Mover Fisicamente al Personaje
    [SerializeField] private float moveSpeed = 5f;
    private CharacterController characterController;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector2 direccion)
    {
        Vector3 movement = new Vector3(
            direccion.x,
            0f,
            direccion.y
        );

        characterController.Move(
    movement * moveSpeed * Time.deltaTime
);
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