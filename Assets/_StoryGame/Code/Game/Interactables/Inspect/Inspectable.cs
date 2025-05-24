using System.Threading.Tasks;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interactables.Inspect
{
    /// <summary>
    /// Например, стеллаж (что не требует открывания, как сейф)
    /// </summary>
    public class Inspectable : Interactable
    {
        public override EInteractableType InteractableType => EInteractableType.Inspect;
        public EInspectState InspectState { get; private set; } = EInspectState.NotInspected;

        private InspectSystem _inspectSystem;

        protected override void ResolveDependencies(IObjectResolver resolver)
        {
            _inspectSystem = resolver.Resolve<InspectSystem>();
            if (_inspectSystem == null)
                throw new System.Exception("InspectSystem is null. " + gameObject.name);
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            await _inspectSystem.Process(this);
        }

        public void SetInspectState(EInspectState state) =>
            InspectState = state;
    }
}
