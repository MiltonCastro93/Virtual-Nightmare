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
        Debug.Log("VAULTING STATE ENTER");
        motor.StartVault();
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        motor.StopVault();
    }


}
