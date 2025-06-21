using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Core.Animations.Messages
{
    public record SetBoolMsg(string Id, bool Value) : IPlayerAnimatorMsg;
}
