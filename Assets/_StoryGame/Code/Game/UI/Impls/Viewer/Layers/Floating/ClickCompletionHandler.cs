using System;
using _StoryGame.Core.UI;
using _StoryGame.Game.Interact.todecor.Decorators.Active;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.Floating
{
    public sealed class ClickCompletionHandler<TResult> where TResult : Enum
    {
        private readonly VisualElement _button;
        private readonly TResult _result;
        private readonly UniTaskCompletionSource<TResult> _completionSource;
        private EventCallback<ClickEvent> _callback;
        private readonly VisualElement _forClearVisualElement;

        public ClickCompletionHandler(
            VisualElement button,
            TResult result,
            UniTaskCompletionSource<TResult> completionSource,
            VisualElement forClearVisualElement = null
        )
        {
            _button = button;
            _result = result;
            _completionSource = completionSource;
            _forClearVisualElement = forClearVisualElement;
        }

        public void Register()
        {
            if (_completionSource.Task.Status.IsCompleted())
                throw new Exception("CompletionSource уже завершён!");

            _callback = _ =>
            {
                _completionSource.TrySetResult(_result);
                _button.UnregisterCallback(_callback);

                Debug.Log($"ClickCompletionHandler - {_result}");
                UniTask.Post(() => _forClearVisualElement?.Clear());
            };

            _button.RegisterCallback(_callback);
        }
    }
}
