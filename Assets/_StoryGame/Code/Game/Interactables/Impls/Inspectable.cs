using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Interactables.Impls.Inspect;
using _StoryGame.Game.Interactables.Interfaces;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _StoryGame.Game.Interactables.Impls
{
    /// <summary>
    /// Объект, который можно осмотреть, а если есть лут - обыскать
    /// </summary>
    public sealed class Inspectable : AInteractable, IInspectable
    {
        public override EInteractableType InteractableType => EInteractableType.Inspect;
        public EInspectState InspectState { get; private set; } = EInspectState.NotInspected;

        private InspectSystem _inspectSystem;

        protected override void ResolveDependencies(IObjectResolver resolver)
        {
            _inspectSystem = resolver.Resolve<InspectSystem>();
            if (_inspectSystem == null)
                throw new Exception("InspectSystem is null. " + gameObject.name);
        }

        public override async UniTask InteractAsync(ICharacter character) =>
            await _inspectSystem.Process(this);

        public void SetInspectState(EInspectState inspectState) =>
            InspectState = inspectState;
    }
}
