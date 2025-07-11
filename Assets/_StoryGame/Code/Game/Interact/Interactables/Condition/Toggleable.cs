using System;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    /// <summary>
    /// Вода которую можно откачать. Веревка, которая сбрасывается в люк. Т.е. есть аним
    /// </summary>
    public sealed class Toggleable : AConditional<ToggleSystem>, IToggleable
    {
        [SerializeField] private EToggleableState defaultState = EToggleableState.NotSet;

        private EToggleableState _currentState = EToggleableState.NotSet; // будет сохраняться между сессиями

        protected override void OnAwake()
        {
            if (defaultState == EToggleableState.NotSet)
                throw new Exception("ToggleableState not set. " + name);

            _currentState = defaultState; //TODO load state
        }

        protected override void Enable()
        {
            if (_currentState == defaultState)
            {
                ConditionChecker = Resolver.Resolve<ConditionChecker>();
                if (ConditionChecker == null)
                {
                    throw new Exception("ConditionChecker is null. " + gameObject.name);
                }

                LOG.Debug(ConditionChecker.CheckConditions(ConditionsData).Success.ToString());
                LOG.Debug("Toggleable: " + name + " Current == Default && condition == false");
                return;
            }


            // тут надо типа проверить условия, обновить стейт, и в зависимости от стейта >>>
            // например, если визибл
        }
    }

    public class ToggleSystem : AInteractSystem<IToggleable>
    {
        public ToggleSystem(InteractSystemDepFlyweight dep) : base(dep)
        {
        }

        protected override UniTask<bool> OnInteractAsync()
        {
            Dep.Log.Debug("Toggleable:  interact" );
            return UniTask.FromResult(true);
        }
    }

    public interface IToggleable : IInteractable
    {
    }

    internal enum EToggleableState
    {
        NotSet = -1,
        Hidden = 0,
        Visible = 1,
        Appearing = 2,
        Disappearing = 3
    }
}
