using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Abstract;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Impls.Use
{
    public sealed class RoomDoor : AUsable
    {
        public override UniTask InteractAsync(ICharacter character)
        {
            return UniTask.CompletedTask;
        }
    }
}
