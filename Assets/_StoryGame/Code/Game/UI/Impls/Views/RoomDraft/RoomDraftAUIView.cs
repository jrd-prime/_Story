using _StoryGame.Core.HSM;
using _StoryGame.Core.HSM.Impls;
using _StoryGame.Core.HSM.Interfaces;
using _StoryGame.Core.HSM.Messages;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.UI.Abstract;
using MessagePipe;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Views.RoomDraft
{
    public sealed class RoomDraftUIView : AUIView<IRoomDraftUIViewModel>
    {
        private VisualElement _visualelem;
        private HSM _hsm;
        private IPublisher<IHSMMsg> hsmMessagePublisher;

        protected override void ResolveDependencies(IObjectResolver resolver)
        {
            hsmMessagePublisher = resolver.Resolve<IPublisher<IHSMMsg>>();
        }

        protected override void InitElements()
        {
            _visualelem = Root.GetVElement<VisualElement>("visualbtn", name);
        }


        private void OnMenuClick2(PointerDownEvent evt)
        {
            Debug.LogWarning("OnMenuClick");
            hsmMessagePublisher.Publish(new ChangeGameStateMessage(EGameStateType.Gameplay));
        }

        protected override void Subscribe()
        {
            _visualelem.RegisterCallback<PointerDownEvent>(OnMenuClick2);
        }
    }

    public interface IRoomDraftUIViewModel : IUIViewModel
    {
    }
}
