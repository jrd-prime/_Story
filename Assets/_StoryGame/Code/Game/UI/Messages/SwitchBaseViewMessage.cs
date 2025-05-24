using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.Interfaces.UI;

namespace _StoryGame.Game.UI.Messages
{
    public record SwitchBaseViewMessage(GameStateType StateType) : IUIViewerMessage
    {
        public string Name => nameof(SwitchBaseViewMessage);
        public GameStateType StateType { get; } = StateType;
    }
}
