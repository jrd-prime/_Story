using R3;
using UnityEngine;

namespace _StoryGame.Core.Character.Common.Interfaces
{
    public interface IFollowable
    {
        ReadOnlyReactiveProperty<Vector3> Position { get; }
    }
}
