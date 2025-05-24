using System;
using _StoryGame.Data.UI;
using _StoryGame.Game.Extensions;
using _StoryGame.Infrastructure.Logging;
using _StoryGame.Infrastructure.Settings;
using R3;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer
{
    public abstract class UIViewerHandlerBase : IDisposable
    {
        protected ISettingsProvider SettingsProvider { get; private set; }
        protected IJLog Log { get; private set; }
        protected VisualElement MainContainer { get; private set; }

        protected readonly CompositeDisposable Disposables = new();

        private readonly VisualElement _layerRoot;
        private readonly IObjectResolver _resolver;

        protected UIViewerHandlerBase(IObjectResolver resolver, VisualElement layerRoot)
        {
            _resolver = resolver;
            _layerRoot = layerRoot;

            Initialize();
        }

        private void Initialize()
        {
            _layerRoot.SetFullScreen();
            MainContainer = _layerRoot.GetVisualElement<VisualElement>(UIConst.MainContainer, GetType().Name);

            ResolveDependencies();
            PreInitialize();
            InitElements();
            Subscribe();
        }

        protected virtual void PreInitialize()
        {
        }

        private void ResolveDependencies()
        {
            SettingsProvider = _resolver.Resolve<ISettingsProvider>();
            Log = _resolver.Resolve<IJLog>();

            ResolveDependencies(_resolver);
        }

        protected virtual void ResolveDependencies(IObjectResolver resolver)
        {
        }

        protected abstract void InitElements();
        protected abstract void Subscribe();
        protected abstract void Unsubscribe();

        protected T GetElement<T>(string id) where T : VisualElement =>
            _layerRoot.GetVisualElement<T>(id, GetType().Name);

        public void Dispose()
        {
            Unsubscribe();
            Disposables?.Dispose();
        }
    }
}
