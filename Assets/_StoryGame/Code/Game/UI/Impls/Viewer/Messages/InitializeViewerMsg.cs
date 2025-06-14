using System.Collections.Generic;
using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.UI.Abstract;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record InitializeViewerMsg(IDictionary<GameStateType, AUIViewBase> Views) : IUIViewerMsg
    {
        public IDictionary<GameStateType, AUIViewBase> Views { get; } = Views;
    }
}
