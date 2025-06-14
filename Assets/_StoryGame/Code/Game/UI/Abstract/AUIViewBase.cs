using _StoryGame.Data.Const;
using _StoryGame.Game.Extensions;
using _StoryGame.Infrastructure.Bootstrap;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Abstract
{
    public abstract class AUIViewBase : MonoBehaviour, IUIView
    {
        [SerializeField] private string viewId;
        [SerializeField] private string viewName;
        [SerializeField] private VisualTreeAsset viewBaseDocument;

        [Inject] protected IObjectResolver Resolver;

        protected VisualElement Root;

        protected readonly CompositeDisposable Disposables = new();
        public TemplateContainer Template { get; private set; }

        private void Awake()
        {
            Template = viewBaseDocument.Instantiate();
            Template.SetFullScreen();
            Template.pickingMode = PickingMode.Ignore;

            Root = Template.GetVisualElement<VisualElement>(UIConst.MainContainer, name);
        }

        public string Id => viewId;

        public abstract void ShowBase();
        public abstract void HideBase();
    }
}
