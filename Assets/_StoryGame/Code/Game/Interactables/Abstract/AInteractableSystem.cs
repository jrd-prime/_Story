using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Interactables.Interfaces;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _StoryGame.Game.Interactables.Abstract
{
    public abstract class AInteractableSystem<TInteractable> : IInteractableSystem
        where TInteractable : class, IInteractable
    {
        protected TInteractable Interactable { get; private set; }
        protected string InteractableId { get; private set; }
        protected IRoom Room { get; private set; }
        protected string LocalizedName { get; private set; }

        protected readonly IJPublisher Publisher;
        protected readonly IJLog Log;
        protected readonly ILocalizationProvider LocalizationProvider;
        protected readonly ISettingsProvider SettingsProvider;
        private readonly IObjectResolver _resolver;

        protected AInteractableSystem(IObjectResolver resolver)
        {
            _resolver = resolver;
            Publisher = _resolver.Resolve<IJPublisher>();
            Log = _resolver.Resolve<IJLog>();
            LocalizationProvider = _resolver.Resolve<ILocalizationProvider>();
            SettingsProvider = _resolver.Resolve<ISettingsProvider>();
        }

        public UniTask<bool> Process(IInteractable interactable)
        {
            Interactable = interactable as TInteractable ??
                           throw new Exception($"Interactable is null as {typeof(TInteractable)}.");

            InteractableId = Interactable.Id;
            Room = Interactable.Room;
            LocalizedName = LocalizationProvider.Localize(Interactable.LocalizationKey, ETable.Words, ETextTransform.Upper);

            return OnProcess();
        }

        protected abstract UniTask<bool> OnProcess();
    }
}
