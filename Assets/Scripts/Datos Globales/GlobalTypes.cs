using UnityEngine;

//Datos como Enums, y Struct Globales para uso de datos desacopla entre scripts
public enum ObstacleType { None, Vault, Climb, Mantle }//Lo uso en PlayerController.cs & CharacterMotor; para el sistema para detectar el tipo de obstaculo
public struct ObstacleData
{
    public ObstacleType Type;
    public Vector3 TopPoint;

    public ObstacleData(ObstacleType type, Vector3 topPoint)
    {
        Type = type;
        TopPoint = topPoint;
    }

}

//posiblemente se creara un struct para el tipo de item