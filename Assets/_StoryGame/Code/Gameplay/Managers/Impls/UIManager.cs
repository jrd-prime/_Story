using _StoryGame.Core.Managers.HSM.Impls;
using _StoryGame.Core.Managers.HSM.Interfaces;
using _StoryGame.Gameplay.Managers.Inerfaces;
using _StoryGame.Infrastructure.Bootstrap;
using _StoryGame.Infrastructure.Logging;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Gameplay.Managers.Impls
{
    public sealed class UIManager : MonoBehaviour, IUIManager, IInitializable
    {
        [SerializeField] private UIViewData[] uiViews;

        private IJLog _log;
        private HSM _hsm;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IJLog log, HSM hsm)
        {
            _log = log;
            _hsm = hsm;
        }

        public void Initialize()
        {
            _log.Info("<color=green>UI MANAGER INITIALIZED</color>");
            _hsm.CurrentState.Subscribe(OnStateChange).AddTo(_disposables);
        }

        private void OnStateChange(IState state)
        {
            if (state == null)
                return;
            _log.Info($"SHOW UI FOR: {state.GetType().Name}");
        }
    }

}
