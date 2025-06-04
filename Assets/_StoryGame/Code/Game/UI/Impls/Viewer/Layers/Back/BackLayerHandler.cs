using _StoryGame.Core.Interfaces.UI;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.Back
{
    public sealed class BackLayerHandler : UIViewerHandlerBase, IUIViewerLayerHandler
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
