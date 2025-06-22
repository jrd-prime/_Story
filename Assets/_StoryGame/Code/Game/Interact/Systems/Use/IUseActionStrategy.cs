using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use
{
    public interface IUseActionStrategy : IStrategy
    {
        UniTask<bool> ExecuteAsync(IUsable usable);
    }
}
