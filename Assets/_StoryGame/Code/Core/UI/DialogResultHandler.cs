using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Core.UI
{
    /// <summary>
    /// Handles dialog results by executing registered callbacks for specific dialog outcomes.
    /// </summary>
    public sealed class DialogResultHandler
    {
        private readonly Dictionary<EDialogResult, Func<UniTask>> _resultHandlers = new();
        private readonly IJLog _log;

        public DialogResultHandler(IJLog log) => _log = log;

        /// <summary>
        /// Добавляет асинхронный коллбэк для определенного результата диалога.
        /// </summary>
        public void AddCallback(EDialogResult dialogResult, Func<UniTask> callback)
        {
            if (_resultHandlers.TryAdd(dialogResult, callback))
                return;

            _log.Warn($"Callback for dialog result {dialogResult} already exists. Overwriting.");
            _resultHandlers[dialogResult] = callback;
        }

        /// <summary>
        /// Обрабатывает результат диалога, выполняя соответствующий асинхронный коллбэк.
        /// </summary>
        public async UniTask HandleResultAsync(EDialogResult result)
        {
            if (_resultHandlers.TryGetValue(result, out var action) && action != null)
            {
                await action.Invoke();
                return;
            }

            _log.Error("No action found for result: " + result);
            await UniTask.CompletedTask;
        }
    }
}
