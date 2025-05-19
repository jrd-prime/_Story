using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Gameplay.Extensions
{
    public static class UIToolkitExtensions
    {
        /// <summary>
        /// Ожидает, пока rootVisualElement и panel будут готовы.
        /// </summary>
        public static async UniTask WaitForReadyAsync(this UIDocument document)
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


        public static T GetVisualElement<T>(this VisualElement root, string id, string name) where T : VisualElement
        {
            var element = root.Q<T>(id);
            if (element == null)
                throw new NullReferenceException($"Element with ID '{id}' not found.");

            return element;
        }

        public static VisualElement SetFullScreen(this VisualElement element)
        {
            var style = element.style;
            style.position = Position.Absolute;
            style.left = style.top = style.right = style.bottom = 0f;
            style.marginLeft = style.marginTop = style.marginRight = style.marginBottom = 0f;
            return element;
        }

        public static VisualElement ContentToCenter(this VisualElement element)
        {
            var style = element.style;
            style.alignContent = Align.Center;
            style.alignItems = Align.Center;
            style.alignSelf = Align.Center;
            style.justifyContent = Justify.Center;
            return element;
        }
    }
}
