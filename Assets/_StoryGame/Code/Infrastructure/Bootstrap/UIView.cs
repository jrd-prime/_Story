using System;
using _StoryGame.Gameplay.Extensions;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Infrastructure.Bootstrap
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIView<TController> : MonoBehaviour, IUIView where TController : IUIViewModel
    {
        [SerializeField] private string viewId;

        public string Id => viewId;

        [Inject] protected TController ViewModel;

        protected VisualElement Root;

        protected readonly CompositeDisposable Disposables = new();

        private async void Start()
        {
            if (ViewModel == null)
                throw new NullReferenceException("ViewModel is null in " + name);

            var uiDocument = GetComponent<UIDocument>();
            await uiDocument.WaitForReadyAsync();

            Root = uiDocument.rootVisualElement ??
                   throw new NullReferenceException("RootVisualElement is null on start in " + name);

            InitElements();
            Subscribe();
        }

        protected abstract void InitElements();
        protected abstract void Subscribe();
    }
}
