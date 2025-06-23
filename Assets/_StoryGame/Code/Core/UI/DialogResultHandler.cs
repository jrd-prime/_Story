using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;

namespace _StoryGame.Core.UI
{
    /// <summary>
    /// Handles dialog results by executing registered callbacks for specific dialog outcomes.
    /// </summary>
    public sealed class DialogResultHandler
    {
        private readonly Dictionary<EDialogResult, Action> _resultHandlers = new();
        private readonly IJLog _log;

        public DialogResultHandler(IJLog log) => _log = log;

        public void AddCallback(EDialogResult dialogResult, Action callback) =>
            _resultHandlers.Add(dialogResult, callback);

        public bool HandleResult(EDialogResult result)
        {
            var action = _resultHandlers[result];

            if (action != null)
            {
                action.Invoke();
                return true;
            }

            _log.Error("No action for result: " + result);
            return false;
        }
    }
}
