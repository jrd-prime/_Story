using R3;
using UnityEngine;

namespace _StoryGame.Core.Character.Common.Interfaces
{
    public interface ICharacter : IMovable
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        object Animator { get; }
        int Health { get; }
        int MaxHealth { get; }
        ReadOnlyReactiveProperty<ECharacterState> State { get; }
        void SetState(ECharacterState state);
    }

    public enum ECharacterState
    {
        Idle,
        MovingToPoint,
        MovingToInteractable,
        Interacting,
        PopUp
    }
}
