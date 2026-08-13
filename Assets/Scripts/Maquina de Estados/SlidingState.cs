using UnityEngine;

public class SlidingState : ICharacterState
{//Deslizamiento iniciado al correr + agacharse
    public bool CanMove => true;//*
    private readonly CharacterMotor motor;
    private readonly CameraController cameraController;
    public SlidingState(CharacterMotor Mymotor, CameraController MyCameraController)
    {
        motor = Mymotor;
        cameraController = MyCameraController;
    }


    public void EnterState()
    {
        motor.StartSlide();
        motor.SetCrouching(true);

        cameraController.SetLookEnabled(false);
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        motor.StopSlide();
        cameraController.SetLookEnabled(true);
    }

}
