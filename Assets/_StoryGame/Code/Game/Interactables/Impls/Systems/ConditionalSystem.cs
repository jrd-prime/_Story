using System;
using System.Collections.Generic;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Managers.Game.Messages;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _StoryGame.Game.Interactables.Impls.Systems
{
    public sealed class ConditionalSystem : AInteractableSystem<Conditional>
    {
        public ConditionalSystem(IObjectResolver resolver) : base(resolver)
        {
        }

        protected override async UniTask<bool> OnProcess()
        {
            var result = Interactable.ConditionalState switch
            {
                EConditionalState.Unknown => throw new Exception("Conditional state is not set on init room! "),
                EConditionalState.Looted => await Looted(),
                EConditionalState.Locked => await Locked(),
                EConditionalState.Unlocked => await Unlocked(),
                _ => false
            };

            Log.Debug($"Interact <color=green>CONDITIONAL INTERACTION END</color>. Result: {result}");
            return result;
        }
        
        
        private async UniTask<bool> Looted()
        {
            Log.Debug("<color=yellow>Looted</color>".ToUpper());
            await UniTask.Yield();
            return true;
        }

        // Сообщение что надо придумать чем открыть (как мысли над головой)
        private async UniTask<bool> Locked()
        {
            Log.Debug("<color=yellow>Locked</color>".ToUpper());
            await UniTask.Yield();
            return true;
        }

        // 
        private async UniTask<bool> Unlocked()
        {
            Log.Debug("<color=yellow>Unlocked</color>".ToUpper());
            var aa = new InspectableLootData(Room.Id, Interactable.Id, null, Interactable.Loot);

            var a = new InspectableData("afdasd", new List<InspectableLootData> { aa });
            Publisher.ForGameManager(new TakeRoomLootMsg(a));
            await UniTask.Yield();
            return true;
        }
    }
}
