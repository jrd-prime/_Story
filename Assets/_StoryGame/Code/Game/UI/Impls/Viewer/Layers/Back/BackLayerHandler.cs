using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.UI.Abstract;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.Back
{
    public sealed class BackLayerHandler : AUIViewerHandlerBase, IUIViewerLayerHandler
    {
        public BackLayerHandler(IObjectResolver resolver, VisualElement layerBack) : base(resolver,layerBack)
        {
        }

        protected override void InitElements()
        {
            
        }

        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
        }
    }
}
