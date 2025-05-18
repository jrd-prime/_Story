namespace _StoryGame.Core.Interfaces
{
    public interface IUpgradable
    {
        int Level { get; }
        int MaxLevel { get; }
        void Upgrade();
    }
}
