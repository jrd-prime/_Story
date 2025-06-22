using System;
using System.Collections.Generic;
using _StoryGame.Data;
using UnityEngine;

namespace _StoryGame.Game.Interact.Systems
{
    /// <summary>
    /// Handles dialog results by executing registered callbacks for specific dialog outcomes.
    /// </summary>
    public sealed class DialogResultHandler
    {
        private readonly Dictionary<EDialogResult, Action> _resultHandlers = new();

        public void AddCallback(EDialogResult dialogResult, Action callback) =>
            _resultHandlers.Add(dialogResult, callback);

        public void HandleResult(EDialogResult result)
        {
            var action = _resultHandlers[result];

            if (action != null)
                action.Invoke();
            else Debug.LogWarning("No action for result: " + result);
        }
    }
}
