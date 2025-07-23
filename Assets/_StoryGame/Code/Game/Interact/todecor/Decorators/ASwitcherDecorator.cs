using System;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Managers;
using _StoryGame.Game.Interact.Switchable.Systems;
using _StoryGame.Game.Interact.todecor.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor.Decorators.Active
{
    public sealed class ASwitcherDecorator : ADecorator, IActiveDecorator
    {
        [Space(10)] [SerializeField] private EGlobalCondition impactOnCondition; // WaterSupply

        [SerializeField] private ESwitchQuestion switchQuestion; // q_turn_on
        [SerializeField] private string requiredItem; // "Crowbar" для вентиля
        [SerializeField] private string missingItemMessage; // "Нужен лом"

        public EGlobalCondition ImpactOnCondition => impactOnCondition;
        public override int Priority => 60;
        public ESwitchQuestion SwitchQuestion => switchQuestion;

        private SimpleSwitchSystem _system;

        [Inject]
        private void Construct(SimpleSwitchSystem system)
        {
            _system = system;
        }

        public async UniTask<bool> ProcessActive(IInteractable interactable)
        {
            if (_system == null)
                throw new Exception("System is null. " + gameObject.name);

            await _system.Process(this, interactable);

            return true;
        }
    }
}
