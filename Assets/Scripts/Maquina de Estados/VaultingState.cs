using UnityEngine;

public class VaultingState : ICharacterState
{
    public bool CanMove => false;
    private readonly CharacterMotor motor;
    public VaultingState(CharacterMotor Mymotor)
    {
        motor = Mymotor;
    }

    public void EnterState()
    {
        Debug.Log("Entré en VaultingState");
        //ObstacleData obstacle = motor.DetectObstacle();
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        Debug.Log("Salí de VaultingState");
    }


}
