using UnityEngine;

public class ClimbingState : ICharacterState
{
    public bool CanMove => false;

    private readonly CharacterMotor motor;
    private readonly Vector3 obstacleTop;
    private readonly Vector3 landingPoint;

    public ClimbingState(CharacterMotor Mymotor, Vector3 GetObstacleTop, Vector3 GetLandingPoint)
    {
        motor = Mymotor;

        obstacleTop = GetObstacleTop;
        landingPoint = GetLandingPoint;
    }

    public void EnterState()
    {
        Debug.Log("Entré en ClimbingState");
        motor.StartVault(obstacleTop, landingPoint);
    }

    public void UpdateState()
    {
        if (motor.IsVaultFinished())
        {
            // PlayerController decidirá el siguiente estado.
        }
    }

    public void ExitState()
    {
        Debug.Log("Salí de ClimbingState");
    }
}
