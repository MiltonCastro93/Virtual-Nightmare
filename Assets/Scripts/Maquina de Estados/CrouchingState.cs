using UnityEngine;

public class CrouchingState : ICharacterState
{//Personaje agachado y movimiento reducido
    public bool CanMove => true;
    private readonly CharacterMotor motor;
    public CrouchingState(CharacterMotor MyMotor) {
        motor = MyMotor;
    }

    public void EnterState()
    {
        motor.SetCrouching(true);
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        motor.SetCrouching(false);
    }

    //expongo un metodo para que el PlayerController determine en que estar volver
    public bool CanExit()
    {
        return motor.CanStandUp();
    }

}
