using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Core.Animations.Messages
{
    public record SetTriggerMsg(string TriggerName) : IPlayerAnimatorMsg;
}
