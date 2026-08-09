using UnityEngine;

public abstract class CharacterHuman : MonoBehaviour
{//proporcionar la base común de un personaje humano
    //Maquina de Estados

    protected CharacterStateMachine stateMachine;

    protected virtual void Awake() {
        stateMachine = new CharacterStateMachine();
    }
    protected virtual void Update() {
        stateMachine.Update();
    }
}
