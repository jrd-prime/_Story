using Cysharp.Threading.Tasks;

namespace _StoryGame.Infrastructure.Bootstrap.Interfaces
{
    public interface IBootable
    {
        string Description { get; }
        bool IsInitialized { get; }
        UniTask InitializeOnBoot();
    }
}
