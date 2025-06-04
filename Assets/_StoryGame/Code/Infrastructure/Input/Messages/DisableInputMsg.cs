using _StoryGame.Infrastructure.Input.Interfaces;

namespace _StoryGame.Infrastructure.Input.Messages
{
    public record DisableInputMsg : IInputMsg
    {
        public string Name => "Disable Input";
    }
}
