using _StoryGame.Core.Character.Common.Interfaces;
using UnityEngine.AI;

namespace _StoryGame.Core.Character.Player.Interfaces
{
    public interface IPlayer : ICharacter, IFollowable
    {
        NavMeshAgent NavMeshAgent { get; }
    }
}
