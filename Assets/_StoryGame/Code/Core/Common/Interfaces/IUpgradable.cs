namespace _StoryGame.Core.Common.Interfaces
{
    public interface IUpgradable
    {
        int Level { get; }
        int MaxLevel { get; }
        void Upgrade();
    }
}
