using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Game.Movement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace _StoryGame.Core.Character.Player.Interfaces
{
    public interface IPlayer : ICharacter, IFollowable, IWalletOwner
    {
        int Energy { get; }
        int MaxEnergy { get; }
        NavMeshAgent NavMeshAgent { get; }
        UniTask MoveToPointAsync(Vector3 position, EDestinationPoint destinationPointType);
        void OnStartInteract();
        void OnEndInteract();

        bool HasEnoughEnergy(int energy);
        void SetEnergy(int energy);
        void AddEnergy(int energy);
        void SpendEnergy(int energy);
    }
}
