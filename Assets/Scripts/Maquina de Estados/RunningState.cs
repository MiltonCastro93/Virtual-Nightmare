using UnityEngine;

public class RunningState : ICharacterState
{
    public bool CanMove => true;

    private readonly CharacterMotor motor;
    public RunningState(CharacterMotor MiMotor) {
        this.motor = MiMotor;
    }

    public void EnterState()
    {
        motor.SetRunning(true);
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        motor.SetRunning(false);
    }

}
