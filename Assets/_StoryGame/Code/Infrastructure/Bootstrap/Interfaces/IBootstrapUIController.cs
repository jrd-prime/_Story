using Cysharp.Threading.Tasks;
using R3;

namespace _StoryGame.Infrastructure.Bootstrap.Interfaces
{
    public interface IBootstrapUIController : IUIViewModel
    {
        ReadOnlyReactiveProperty<string> LoadingText { get; }
        ReadOnlyReactiveProperty<float> Opacity { get; }
        Observable<Unit> OnClear { get; }
        void SetLoadingText(string value);
        UniTask FadeOutAsync(float durationInSeconds);
    }
}
