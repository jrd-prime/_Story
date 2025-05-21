using System;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace _StoryGame.Core.Character.Common.Interfaces
{
    public interface ICharacterInteractor
    {
        void AnimateWithTrigger(string triggerName, string animationStateName, Action onAnimationComplete);
        void SetDestinationPoint(Vector3 destination);
        NavMeshAgent NavMeshAgent { get; }
        ReadOnlyReactiveProperty<Vector3> DestinationPoint { get; }
        int MaxHealth { get; }
        int Health { get; }
        string Description { get; }
        string Name { get; }
        string Id { get; }
        void SetNavMeshAgent(NavMeshAgent navMeshAgent);
    }
}
