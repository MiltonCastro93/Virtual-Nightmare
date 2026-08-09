
public interface ICharacterState {
    void EnterState();
    void UpdateState();
    void ExitState();

    bool CanMove { get; }
}
