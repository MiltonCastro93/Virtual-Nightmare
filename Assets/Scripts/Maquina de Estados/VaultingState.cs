using UnityEngine;

public class VaultingState : ICharacterState
{
    public bool CanMove => false;

    private readonly CharacterMotor motor;
    private readonly Vector3 obstacleTop;
    private readonly Vector3 landingPoint;

    public VaultingState( CharacterMotor Mymotor, Vector3 GetObstacleTop, Vector3 GetLandingPoint)
    {
        motor = Mymotor;

        obstacleTop = GetObstacleTop;
        landingPoint = GetLandingPoint;
    }

    public void EnterState()
    {
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
        Debug.Log("Salí de VaultingState");
    }
}
