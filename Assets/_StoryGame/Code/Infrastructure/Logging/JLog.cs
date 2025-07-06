using System.Diagnostics;
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
            UnityEngine.Debug.Log($"{GetCallerClassName()}<color=grey>[I]</color> : {message}");
        }

        public void Debug(string message)
        {
            if (!enableDebug) return;
            UnityEngine.Debug.Log($"{GetCallerClassName()}<color=white>[D]</color> : {message}");
        }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError($"{GetCallerClassName()}<color=red>[E]</color> : {message}");
        }

        public void Warn(string message)
        {
            UnityEngine.Debug.LogWarning($"{GetCallerClassName()}<color=yellow>[W]</color>  : {message}");
        }

        private static string GetCallerClassName()
        {
            var stackTrace = new StackTrace();
            for (var i = 2; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                var method = frame.GetMethod();
                var type = method.DeclaringType;

                if (type == null)
                    continue;

                // Пропускаем сгенерированные async/iterator классы
                if (type.Name.StartsWith('<') && type.DeclaringType != null)
                    return $"<color=orange>[ {type.DeclaringType.Name} ]</color>";

                return $"<color=orange>[ {type.Name} ]</color>";
            }

            return "<color=orange>[ Unknown ]</color>";
        }
    }
}
