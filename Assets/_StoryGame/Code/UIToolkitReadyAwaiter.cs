using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame
{
    public static class UIToolkitReadyAwaiter
    {
        /// <summary>
        /// Ожидает, пока rootVisualElement и panel будут готовы.
        /// </summary>
        public static async UniTask WaitForReadyAsync(UIDocument document)
        {
            if (document == null)
            {
                Debug.LogError("[UIToolkitReadyAwaiter] UIDocument is null!");
                return;
            }

            // Ждём, пока rootVisualElement появится
            await UniTask.WaitUntil(() => document.rootVisualElement != null);

            // Ждём, пока panel станет доступна (чаще всего тут и падает)
            await UniTask.WaitUntil(() => document.rootVisualElement.panel != null);
            // Log.Warn($"<color=green><b>UI Toolkit is ready!</b> {document.name}</color>");
        }
    }
}
