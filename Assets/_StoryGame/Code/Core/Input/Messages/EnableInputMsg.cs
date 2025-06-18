using _StoryGame.Core.Input.Interfaces;

namespace _StoryGame.Core.Input.Messages
{
    public record EnableInputMsg : IInputMsg
    {
        public string Name => "Enable Input";
    }
}
