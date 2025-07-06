using Cysharp.Threading.Tasks;

namespace _StoryGame.Core.Common.Interfaces
{
    public interface IBootable
    {
        string Description { get; }
        bool IsInitialized { get; }
        UniTask InitializeOnBoot();
    }
}
