using _StoryGame.Infrastructure.Input.Interfaces;

namespace _StoryGame.Infrastructure.Input.Messages
{
    public record DisableInputMessage : IInputMessage
    {
        public string Name => "Disable Input";
    }
}
