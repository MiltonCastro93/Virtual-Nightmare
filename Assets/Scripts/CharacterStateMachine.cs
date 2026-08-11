using UnityEngine;

public class CharacterStateMachine
{//Única responsabilidad: administrar cuál es el estado actual del personaje.
    public ICharacterState currentState { get; private set; }
    //podria integrar una variable del mismo tipo asi al momento de salir, puedo saber si estas parado o agachado
    public void ChangeState(ICharacterState newState) {
        if (newState == null) return;

        currentState?.ExitState();
        this.currentState = newState;
        currentState?.EnterState();
    }

    public void Update() {
        currentState?.UpdateState();
    }

}
