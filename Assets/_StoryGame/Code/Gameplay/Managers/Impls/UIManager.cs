using System;
using System.Collections.Generic;
using _StoryGame.Core.Managers.HSM.Impls;
using _StoryGame.Core.Managers.HSM.Impls.States;
using _StoryGame.Gameplay.Managers.Inerfaces;
using _StoryGame.Gameplay.UI.Impls;
using _StoryGame.Gameplay.UI.Impls.Viewer;
using _StoryGame.Infrastructure.Logging;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Gameplay.Managers.Impls
{
    public sealed class UIManager : MonoBehaviour, IUIManager, IInitializable
    {
        [SerializeField] private UIViewer viewer;
        [SerializeField] private UIViewData[] baseViews;

        private IJLog _log;
        private HSM _hsm;

        private GameStateType _currentBaseView;

        private readonly Dictionary<GameStateType, UIViewBase> _viewsCache = new();
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IJLog log, HSM hsm)
        {
            _log = log;
            _hsm = hsm;
        }

        public void Initialize()
        {
            if (!viewer)
                throw new NullReferenceException("Viewer is null. " + nameof(UIManager));

            if (baseViews == null || baseViews.Length == 0)
                throw new NullReferenceException("No ui views. " + nameof(UIManager));

            foreach (var uiView in baseViews)
                _viewsCache.Add(uiView.type, uiView.view);

            _hsm.CurrentStateType
                .Subscribe(OnStateChange)
                .AddTo(_disposables);
        }

        private void Start() => viewer.Initialize(_viewsCache);

        private async void OnStateChange(GameStateType state)
        {
            if (state == GameStateType.NotSet)
                return;

            await UniTask.Yield();

            _log.Info($"SHOW UI FOR: {state}");

            _currentBaseView = state;
            viewer.SwitchTo(state);
        }
    }
}
