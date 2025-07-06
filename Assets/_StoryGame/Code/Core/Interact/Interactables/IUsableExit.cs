using _StoryGame.Core.Room;
using _StoryGame.Game.Interact.Interactables.Use;
using _StoryGame.Game.Room.Abstract;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface IUsableExit : IUsable
    {
        string ExitQuestionLocalizationKey { get; }
        ERoom RoomType { get; }
        ERoom TransitionToRoom { get; }
        EDoorAction DoorAction { get; }
    }
}
