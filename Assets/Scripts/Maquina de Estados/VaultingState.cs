using UnityEngine;

public class VaultingState : ICharacterState
{
    public bool CanMove => false;
    private readonly CharacterMotor motor;
    private readonly Vector3 obstacleTop;

    public VaultingState(CharacterMotor Mymotor, Vector3 GetObstacleTop)
    {
        motor = Mymotor;
        obstacleTop = GetObstacleTop;
    }

    public void EnterState()
    {
        motor.StartVault(obstacleTop);
    }

    public void UpdateState()
    {
        if (motor.IsVaultFinished())
        {
            // El PlayerController decidirá el siguiente estado.
        }
    }

    public void ExitState()
    {
        Debug.Log("Salí de VaultingState");
    }


}
