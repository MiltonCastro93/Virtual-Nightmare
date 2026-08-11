
public interface ICharacterState {
    void EnterState();
    void UpdateState();
    void ExitState();

    //Para un juego estilo Outlast, yo pienso que CanMove no signifique "puede moverse físicamente" sino:
    //"Este estado permite que el jugador controle libremente el movimiento." durante un vault: "VaultingState" -> CanMove = false
    //porque una animación controla al personaje en bool = false.
    bool CanMove { get; }
}
