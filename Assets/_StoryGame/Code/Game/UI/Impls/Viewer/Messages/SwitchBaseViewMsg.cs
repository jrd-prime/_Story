using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.UI.Interfaces;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record SwitchBaseViewMsg(GameStateType StateType) : IUIViewerMsg
    {
        public GameStateType StateType { get; } = StateType;
    }
}
