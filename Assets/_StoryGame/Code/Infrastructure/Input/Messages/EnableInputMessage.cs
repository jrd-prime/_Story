using _StoryGame.Infrastructure.Input.Interfaces;

namespace _StoryGame.Infrastructure.Input.Messages
{
    public record EnableInputMessage : IInputMessage
    {
        public string Name => "Enable Input";
    }
}
