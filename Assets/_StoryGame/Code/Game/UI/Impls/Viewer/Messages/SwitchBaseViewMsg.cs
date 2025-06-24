using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.UI.Interfaces;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record SwitchBaseViewMsg(EGameStateType StateType) : IUIViewerMsg
    {
        public EGameStateType StateType { get; } = StateType;
    }
}
