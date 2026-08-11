using UnityEngine;

public class FallingState : ICharacterState
{//Personaje en el aire descendiendo
    public bool CanMove => true;//*

    public void EnterState()
    {
        throw new System.NotImplementedException();
    }
    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }
    public void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
