using System;
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
        protected VisualElement MainContainer;

        protected readonly CompositeDisposable Disposables = new();

        private async void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            await uiDocument.WaitForReadyAsync();

            Root = uiDocument.rootVisualElement ??
                   throw new NullReferenceException("RootVisualElement is null on start in " + name);

            MainContainer = Root.GetVisualElement<VisualElement>("main-container", name);
        }

        public string Id => viewId;

        public abstract void ShowBase();
        public abstract void HideBase();
    }
}
