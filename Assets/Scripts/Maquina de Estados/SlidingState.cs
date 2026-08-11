using UnityEngine;

public class SlidingState : ICharacterState
{//Deslizamiento iniciado al correr + agacharse
    public bool CanMove => false;//*
    private readonly CharacterMotor motor;
    public SlidingState(CharacterMotor Mymotor)
    {
        motor = Mymotor;
    }


    public void EnterState()
    {
        motor.StartSlide();
    }

    public void UpdateState()
    {
        //Mas Adelante: Comprobar si el Slide Termino!
    }

    public void ExitState()
    {
        motor.StopSlider();
    }

}
