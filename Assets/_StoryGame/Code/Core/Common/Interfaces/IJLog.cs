namespace _StoryGame.Core.Common.Interfaces
{
    public interface IJLog
    {
        void Info(string message);
        void Error(string message);
        void Warn(string message);
        void Debug(string message);
    }
}
