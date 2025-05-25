using System;
using _StoryGame.Game.Interactables.Inspect;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers
{
    public sealed class ClickCompletionHandler<T>
    {
        private readonly VisualElement _button;
        private readonly DialogResult _result;
        private readonly UniTaskCompletionSource<T> _completionSource;
        private EventCallback<ClickEvent> _callback;
        private readonly VisualElement _holderVisualElement;

        public ClickCompletionHandler(VisualElement button, DialogResult result,
            UniTaskCompletionSource<T> completionSource, VisualElement holderVisualElement)
        {
            _button = button;
            _result = result;
            _completionSource = completionSource;
            _holderVisualElement = holderVisualElement;
        }

        public void Register()
        {
            if (_completionSource.Task.Status.IsCompleted())
                throw new Exception("CompletionSource уже завершён!");
            
            _callback = _ =>
            {
                _completionSource.TrySetResult((T)(object)_result);
                _button.UnregisterCallback(_callback);

                Debug.Log($"ClickCompletionHandler - {_result}");
                UniTask.Post(() => _holderVisualElement.Clear());
            };

            _button.RegisterCallback(_callback);
        }
    }
}
