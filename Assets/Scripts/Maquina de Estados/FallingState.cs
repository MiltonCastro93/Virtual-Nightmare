using UnityEngine;

public class FallingState : ICharacterState
{//Personaje en el aire descendiendo
    public bool CanMove => true;//*
    private readonly CharacterMotor motor;
    public FallingState(CharacterMotor MyMotor)
    {
        motor = MyMotor;
    }

    public void EnterState()
    {

    }
    public void UpdateState()
    {

    }
    public void ExitState()
    {

    }
}
