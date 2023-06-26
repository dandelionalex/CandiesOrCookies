using PickMaster.Enums;

namespace PickMaster.DI.Signals
{
    public class GameStateChangedSignal
    {
        public GameState GameState { get; }

        public GameStateChangedSignal(GameState gameState)
        {
            this.GameState = gameState;
        }
    }
}