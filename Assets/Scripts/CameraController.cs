using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{//Controlar el comportamiento de la cámara
    // Controlar el comportamiento de la cámara
    [Header("GameObject Padre")]
    [SerializeField] private Transform player;

    [Header("Variables de la Cámara")]
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float angleMax = 45f;

    [Header("Inclinación")]
    [SerializeField] private float angleLeanMax = 20f;

    private float rotationY;
    private float rotationX;
    private float rotationZ;

    public void Look(Vector2 input)
    {
        float mouseX = input.x * sensitivity;
        float mouseY = input.y * sensitivity;

        // Rotación horizontal del jugador
        player.Rotate(
            Vector3.up * mouseX * Time.deltaTime
        );

        // Rotación vertical de la cámara
        rotationX -= mouseY * Time.deltaTime;

        rotationX = Mathf.Clamp(
            rotationX,
            -angleMax,
            angleMax
        );

        ApplyRotation();
    }

    public void LeanLeft()
    {
        rotationZ = angleLeanMax;
    }

    public void LeanRight()
    {
        rotationZ = -angleLeanMax;
    }

    public void ResetLean()
    {
        rotationZ = 0f;
    }

    public void LookBack()
    {
        rotationY = 180f;
    }

    public void ResetLookBack()
    {
        rotationY = 0f;
    }


    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(
            rotationX,
            rotationY,
            rotationZ
        );
    }

}
/*
├── Look()
├── LeanLeft()
├── LeanRight()
├── ResetLean()
├── SetLookSensitivity()//posiblemente
└── SetFOV()
*/