namespace _StoryGame.Core.Interact.Interactables
{
    public interface IUsableExit : IUsable
    {
        string ExitQuestionLocalizationKey { get; }
    }
}
