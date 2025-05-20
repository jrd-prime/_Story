using _StoryGame.Gameplay.UI.Interfaces;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls.Viewer.Layers
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
