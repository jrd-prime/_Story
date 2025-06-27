using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.HSM;
using _StoryGame.Core.HSM.Impls;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.UI;
using _StoryGame.Game.UI.Abstract;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Game.Managers
{
    public sealed class UIManager : MonoBehaviour, IUIManager, IInitializable
    {
        [SerializeField] private UIViewData[] baseViews;

        private EGameStateType _currentBaseView;
        private IJPublisher _publisher;
        private IJLog _log;
        private HSM _hsm;

        private readonly Dictionary<EGameStateType, AUIViewBase> _viewsCache = new();
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IJPublisher publisher, IJLog log, HSM hsm)
        {
            _publisher = publisher;
            _log = log;
            _hsm = hsm;
        }

        public void Initialize()
        {
            if (baseViews == null || baseViews.Length == 0)
                throw new NullReferenceException("No ui views. " + nameof(UIManager));

            foreach (var uiView in baseViews)
            {
                _log.Debug($"Register view: {uiView.type}");
                _viewsCache.Add(uiView.type, uiView.view);
            }


            _hsm.CurrentStateType
                .Subscribe(OnStateChange)
                .AddTo(_disposables);
        }

        private void Start()
        {
            _log.Debug("UIManager.Start");
            Debug.Log("UIManager publish InitializeViewerMsg");
            _publisher.ForUIViewer(new InitializeViewerMsg(_viewsCache));
        }

        private async void OnStateChange(EGameStateType state)
        {
            if (state == EGameStateType.NotSet)
                return;

            await UniTask.Yield();

            _currentBaseView = state;

            _publisher.ForUIViewer(new SwitchBaseViewMsg(state));
        }
    }
}
