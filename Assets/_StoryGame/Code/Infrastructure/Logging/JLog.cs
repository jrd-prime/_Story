using _StoryGame.Core.Common.Interfaces;
using UnityEngine;

namespace _StoryGame.Infrastructure.Logging
{
    public class JLog : MonoBehaviour, IJLog
    {
        [SerializeField] private bool enableDebug;
        [SerializeField] private bool enableInfo;

        public void Info(string message)
        {
            if (!enableInfo) return;
            UnityEngine.Debug.Log(message);
        }

        public void Debug(string message)
        {
            if (!enableDebug) return;
            UnityEngine.Debug.Log(message);
        }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public void Warn(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}
