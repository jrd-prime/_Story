using _StoryGame.Infrastructure.Input.Interfaces;

namespace _StoryGame.Infrastructure.Input.Messages
{
    public record EnableInputMsg : IInputMsg
    {
        public string Name => "Enable Input";
    }
}
