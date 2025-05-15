using Cysharp.Threading.Tasks;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public interface IBootable
    {
        string Description { get; }
        bool IsInitialized { get; }
        UniTask InitializeOnBoot();
    }
}
