using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Data.Loot;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Movement;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace _StoryGame.Core.Character.Player.Interfaces
{
    public interface IPlayer : ICharacter, IFollowable, IWalletOwner
    {
        ReadOnlyReactiveProperty<int> Energy { get; }
        ReadOnlyReactiveProperty<int> MaxEnergy { get; }
        NavMeshAgent NavMeshAgent { get; }
        UniTask MoveToPointAsync(Vector3 position, EDestinationPoint destinationPointType);
        void OnStartInteract();
        void OnEndInteract();

        bool HasEnoughEnergy(int energy);
        void SetEnergy(int energy);
        void AddEnergy(int energy);
        void SpendEnergy(int energy);
        void AddNote(PreparedLootVo preparedLootVo);
        void AddItemToWallet(ACurrencyData itemData, int amount);
        void SetPosition(Vector3 value);
    }
}
