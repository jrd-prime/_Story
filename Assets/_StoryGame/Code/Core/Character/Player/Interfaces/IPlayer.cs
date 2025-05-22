using System.Threading.Tasks;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Movement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace _StoryGame.Core.Character.Player.Interfaces
{
    public interface IPlayer : ICharacter, IFollowable
    {
        NavMeshAgent NavMeshAgent { get; }
        UniTask MoveToPointAsync(Vector3 position, EDestinationPoint destinationPointType);
        void OnStartInteract();
        void OnEndInteract();
    }
}
