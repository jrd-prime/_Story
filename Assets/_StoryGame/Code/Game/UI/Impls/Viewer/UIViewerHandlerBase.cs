using System;
using _StoryGame.Core.Interfaces.Managers;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Main;
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

        protected IGameManager GameManager { get; private set; }

        protected readonly CompositeDisposable Disposables = new();

        private readonly VisualElement _layerRoot;
        private readonly IObjectResolver _resolver;
        protected UISettings UISettings;

        protected UIViewerHandlerBase(IObjectResolver resolver, VisualElement layerRoot)
        {
            _resolver = resolver;
            _layerRoot = layerRoot;

            GameManager = _resolver.Resolve<IGameManager>();

            Initialize();
        }

        private void Initialize()
        {
            _layerRoot.SetFullScreen();
            MainContainer = _layerRoot.GetVisualElement<VisualElement>(UIConst.MainContainer, GetType().Name);

            ResolveDependencies();

            UISettings = SettingsProvider.GetSettings<UISettings>();

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
