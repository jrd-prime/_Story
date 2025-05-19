using _StoryGame.Core.Managers.HSM.Impls;
using _StoryGame.Core.Managers.HSM.Impls.States;
using _StoryGame.Core.Managers.HSM.Interfaces;
using _StoryGame.Core.Managers.HSM.Messages;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Infrastructure.Bootstrap;
using MessagePipe;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls.Menu
{
    public sealed class MenuUIView : UIView<IMenuUIViewModel>
    {
        private VisualElement _visualelem;
        private HSM _hsm;
        private IPublisher<IHSMMessage> hsmMessagePublisher;

        protected override void ResolveDependencies(IObjectResolver resolver)
        {
            hsmMessagePublisher = resolver.Resolve<IPublisher<IHSMMessage>>();
        }

        protected override void InitElements()
        {
            _visualelem = Root.GetVisualElement<VisualElement>("visualbtn", name);
        }


        private void OnMenuClick2(PointerDownEvent evt)
        {
            Debug.LogWarning("OnMenuClick");
            hsmMessagePublisher.Publish(new ChangeGameStateMessage(GameStateType.Gameplay));
        }

        protected override void Subscribe()
        {
            _visualelem.RegisterCallback<PointerDownEvent>(OnMenuClick2);
        }
    }

    public interface IMenuUIViewModel : IUIViewModel
    {
    }
}
