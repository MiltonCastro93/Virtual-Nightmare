using UnityEngine;

public class CharacterStateMachine
{//Única responsabilidad: administrar cuál es el estado actual del personaje.
    private ICharacterState currentState;
    public void ChangeState(ICharacterState newState) {
        currentState?.ExitState();
        this.currentState = newState;
        currentState?.EnterState();
    }

    public void Update() {
        currentState?.UpdateState();
    }

}
