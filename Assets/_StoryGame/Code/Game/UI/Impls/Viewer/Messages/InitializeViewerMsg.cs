using System.Collections.Generic;
using _StoryGame.Core.HSM;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.UI.Abstract;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record InitializeViewerMsg(IDictionary<EGameStateType, AUIViewBase> Views) : IUIViewerMsg
    {
        public IDictionary<EGameStateType, AUIViewBase> Views { get; } = Views;
    }
}
