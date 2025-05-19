using _StoryGame.Data.UI;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Infrastructure.Bootstrap;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls
{
    public abstract class UIViewBase : MonoBehaviour, IUIView
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
