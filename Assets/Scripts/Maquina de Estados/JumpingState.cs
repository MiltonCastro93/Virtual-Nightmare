using UnityEngine;

public class JumpingState : ICharacterState
{//Inicio y fase ascendente del salto

    //Los * los vamos a decidir según cómo quiero que se sienta el juego. (JumpingState; FallingState; SlidingState)
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
