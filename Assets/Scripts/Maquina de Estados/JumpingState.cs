using UnityEngine;

public class JumpingState : ICharacterState
{//Inicio y fase ascendente del salto

    //Los * los vamos a decidir según cómo quiero que se sienta el juego. (JumpingState; FallingState; SlidingState)
    public bool CanMove => true;//*
    private readonly CharacterMotor motor;
    public JumpingState(CharacterMotor MyMotor)
    {
        motor = MyMotor;
    }

    public void EnterState()
    {
        motor.Jump();
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {

    }

}
