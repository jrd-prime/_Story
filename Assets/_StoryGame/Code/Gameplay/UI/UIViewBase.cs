using _StoryGame.Infrastructure.Bootstrap;
using UnityEngine;

namespace _StoryGame.Gameplay.UI
{
    public abstract class UIViewBase : MonoBehaviour, IUIView
    {
        [SerializeField] private string viewId;

        public string Id => viewId;

        public abstract void ShowBase();
        public abstract void HideBase();
    }
}
