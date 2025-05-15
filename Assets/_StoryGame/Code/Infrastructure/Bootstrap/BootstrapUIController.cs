using System;
using Cysharp.Threading.Tasks;
using R3;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public sealed class BootstrapUIController : IBootstrapUIController, IDisposable
    {
        public ReadOnlyReactiveProperty<string> LoadingText => _loadingText;
        public ReadOnlyReactiveProperty<float> Opacity => _opacity;
        public Observable<Unit> OnClear => _onClear;

        private readonly ReactiveProperty<string> _loadingText = new();
        private readonly ReactiveProperty<float> _opacity = new(1f);
        private readonly Subject<Unit> _onClear = new();

        public void SetLoadingText(string value) =>
            _loadingText.Value = !string.IsNullOrEmpty(value) ? value : "Not set";

        public async UniTask FadeOutAsync(float durationInSeconds)
        {
            Clear();
            const float fadeStep = 0.01f;
            var tickDelay = durationInSeconds / (1 / fadeStep);

            while (_opacity.Value > 0)
            {
                _opacity.Value -= fadeStep;
                await UniTask.WaitForSeconds(tickDelay);
            }
        }

        private void Clear() => _onClear.OnNext(Unit.Default);

        public void Dispose()
        {
            _loadingText?.Dispose();
            _opacity?.Dispose();
            _onClear?.Dispose();
        }
    }
}
