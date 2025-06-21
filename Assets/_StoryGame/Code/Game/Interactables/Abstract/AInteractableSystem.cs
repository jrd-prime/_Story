using System;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.SO.Interactables;
using _StoryGame.Game.Interactables.Impls;
using _StoryGame.Game.Interactables.Interfaces;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _StoryGame.Game.Interactables.Abstract
{
    public abstract class AInteractableSystem<TInteractable> : IInteractableSystem
        where TInteractable : class, IInteractable
    {
        protected string InteractableId { get; private set; }
        protected IRoom Room { get; private set; }
        protected string LocalizedName { get; private set; }

        protected readonly IJPublisher Publisher;
        protected readonly IJLog Log;
        protected readonly ILocalizationProvider LocalizationProvider;
        protected readonly ISettingsProvider SettingsProvider;
        protected readonly IObjectResolver _resolver;
        protected readonly InteractableSystemTipData InteractableSystemTipData;

        private TInteractable Interactable;

        protected AInteractableSystem(IObjectResolver resolver)
        {
            _resolver = resolver;
            Publisher = _resolver.Resolve<IJPublisher>();
            Log = _resolver.Resolve<IJLog>();
            LocalizationProvider = _resolver.Resolve<ILocalizationProvider>();
            SettingsProvider = _resolver.Resolve<ISettingsProvider>();

            InteractableSystemTipData = SettingsProvider.GetSettings<InteractableSystemTipData>();
        }


        public UniTask<bool> Process(IInteractable interactable)
        {
            Interactable = interactable as TInteractable ??
                           throw new Exception($"Interactable is null as {typeof(TInteractable)}.");

            InteractableId = Interactable.Id;
            Room = Interactable.Room;
            LocalizedName =
                LocalizationProvider.Localize(Interactable.LocalizationKey, ETable.Words, ETextTransform.Upper);

            OnPreProcess(Interactable);

            return OnProcessAsync();
        }

        protected abstract void OnPreProcess(TInteractable interactable);

        protected abstract UniTask<bool> OnProcessAsync();

        protected void SendBoolToPlayerAnimator(string key, bool value) =>
            Publisher.ForPlayerAnimator(new SetBoolMsg(key, value));
    }
}
