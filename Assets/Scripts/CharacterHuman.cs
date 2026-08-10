using UnityEngine;

public abstract class CharacterHuman : MonoBehaviour
{//proporcionar la base común de un personaje humano
    //Maquina de Estados
    protected CharacterStateMachine stateMachine;
    protected CharacterMotor motor;

    //Estados Base de la locomocion
    protected IdleState idleState;
    protected WalkingState walkingState;
    protected RunningState runningState;

    protected virtual void Awake() {
        stateMachine = new CharacterStateMachine();
        motor = GetComponent<CharacterMotor>();

        idleState = new IdleState();
        walkingState = new WalkingState();
        runningState = new RunningState(motor);

        stateMachine.ChangeState(idleState);
    }
    protected virtual void Update() {
        stateMachine.Update();
    }
}
