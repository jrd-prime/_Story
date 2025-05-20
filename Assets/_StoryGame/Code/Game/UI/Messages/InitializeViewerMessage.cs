using System.Collections.Generic;
using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.UI.Impls;

namespace _StoryGame.Game.UI.Messages
{
    public record InitializeViewerMessage(IDictionary<GameStateType, UIViewBase> Views) : IUIViewerMessage
    {
        public string Name => nameof(InitializeViewerMessage);
        public IDictionary<GameStateType, UIViewBase> Views { get; } = Views;
    }
}
