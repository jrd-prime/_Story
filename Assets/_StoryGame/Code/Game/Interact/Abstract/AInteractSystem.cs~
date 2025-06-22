using System;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Infrastructure.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Abstract
{
    public abstract class AInteractSystem<TInteractable> : IInteractableSystem
        where TInteractable : class, IInteractable
    {
        protected string InteractableId { get; private set; }
        protected IRoom Room { get; private set; }
        protected string LocalizedName { get; private set; }

        protected readonly InteracSystemDepFlyweight SystemDep;

        private TInteractable Interactable;

        protected AInteractSystem(InteracSystemDepFlyweight systemDep) => SystemDep = systemDep;

        public UniTask<bool> Process(IInteractable interactable)
        {
            Interactable = interactable as TInteractable ??
                           throw new Exception($"Interact is null as {typeof(TInteractable)}.");

            InteractableId = Interactable.Id;
            Room = Interactable.Room;
            LocalizedName =
                SystemDep.LocalizationProvider.Localize(Interactable.LocalizationKey, ETable.Words,
                    ETextTransform.Upper);

            OnPreProcess(Interactable);

            return OnProcessAsync();
        }

        protected abstract void OnPreProcess(TInteractable interactable);

        protected abstract UniTask<bool> OnProcessAsync();
    }
}
