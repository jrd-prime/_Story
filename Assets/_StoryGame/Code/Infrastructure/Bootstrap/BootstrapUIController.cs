using System;
using _StoryGame.Infrastructure.Logging;
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

        private bool _isDisposed;
        private bool _isFadingOut;
        private readonly IJLog _log;

        public BootstrapUIController(IJLog log)
        {
            _log = log;

            if (_log == null)
                throw new NullReferenceException("Logger is null.");
        }

        public void SetLoadingText(string value)
        {
            if (_isDisposed)
            {
                _log.Warn("[BootstrapUIController] Attempt to set loading text after disposal.");
                return;
            }

            _loadingText.Value = string.IsNullOrWhiteSpace(value) ? "Not set" : value;
        }

        public async UniTask FadeOutAsync(float durationInSeconds)
        {
            if (_isDisposed)
            {
                _log.Warn("[BootstrapUIController] Attempt to fade out after disposal.");
                return;
            }

            if (_isFadingOut)
                return;

            _isFadingOut = true;

            try
            {
                Clear();

                const float fadeStep = 0.01f;
                var tickDelay = durationInSeconds / (1f / fadeStep);

                while (_opacity.Value > 0f)
                {
                    _opacity.Value = Math.Max(0f, _opacity.Value - fadeStep);
                    await UniTask.WaitForSeconds(tickDelay);
                }

                _opacity.Value = 0f;
            }
            catch (Exception ex)
            {
                _log.Error($"[BootstrapUIController] FadeOutAsync failed: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                _isFadingOut = false;
            }
        }

        private void Clear()
        {
            if (_isDisposed)
            {
                _log.Warn("[BootstrapUIController] Attempt to clear after disposal.");
                return;
            }

            _onClear.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            try
            {
                _loadingText?.Dispose();
                _opacity?.Dispose();
                _onClear?.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error($"[BootstrapUIController] Error during Dispose: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
