using UnityEngine;

public class SlidingState : ICharacterState
{//Deslizamiento iniciado al correr + agacharse
    public bool CanMove => true;//*
    private readonly CharacterMotor motor;
    public SlidingState(CharacterMotor Mymotor)
    {
        motor = Mymotor;
    }


    public void EnterState()
    {
        motor.StartSlide();
        motor.SetCrouching(true);
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        motor.StopSlide();
    }

}
