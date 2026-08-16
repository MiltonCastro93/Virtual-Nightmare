using UnityEngine;

public class ClimbingState : ICharacterState
{
    public bool CanMove => false;

    private readonly CharacterMotor motor;

    public ClimbingState(CharacterMotor Mymotor)
    {
        motor = Mymotor;
    }

    public void EnterState()
    {
        Debug.Log("Entré en ClimbingState");
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
        Debug.Log("Salí de ClimbingState");
    }
}
