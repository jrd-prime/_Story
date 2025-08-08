using System;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Game.Interact.todecor.Decorators.Active;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.Interact.todecor.Impl.DeviceSystems
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class ADeviceUI : MonoBehaviour
    {
        private VisualElement _root;
        protected VisualElement MainContainer;
        private InteractSystemDepFlyweight Dep;

        [Inject]
        private void Construct(InteractSystemDepFlyweight dep)
        {
            Debug.LogWarning("<color=yellow>Construct called for " + gameObject.name + " " + GetType().Name);
            Dep = dep;
        }

        private void Awake()
        {
            Debug.LogWarning("Awake called for " + gameObject.name + " " + GetType().Name);

            var doc = GetComponent<UIDocument>()
                      ?? throw new NullReferenceException("UIDocument not found");

            _root = doc.rootVisualElement
                    ?? throw new NullReferenceException("rootVisualElement not found");

            MainContainer = _root.Q<VisualElement>("main-container")
                            ?? throw new NullReferenceException("main-container not found");

            OnAwake();
            InitializeElements();
        }

        private void Start()
        {
            if (Dep == null)
                throw new NullReferenceException("Dep is null. " + gameObject.name);

            LocalizeElements();

            HidePanel();
        }

        public void ShowPanel<T>(DevDa<T> da) where T : Enum
        {
            if (Dep == null)
                throw new NullReferenceException("Dep is null. " + gameObject.name);

            _root.style.display = DisplayStyle.Flex;
            OnShowPanel(da);
        }

        public void HidePanel()
        {
            _root.style.display = DisplayStyle.None;
            OnHidePanel();
        }

        protected abstract void OnShowPanel<T>(DevDa<T> da) where T : Enum;


        protected virtual void OnHidePanel()
        {
        }

        protected string Localize(string key, ETable table, ETextTransform textTransform = ETextTransform.None) =>
            Dep.L10n.Localize(key, table, textTransform);


        protected virtual void OnAwake()
        {
        }

        protected abstract void InitializeElements();
        protected abstract void LocalizeElements();
    }

    public record DevDa<T>(UniTaskCompletionSource<T> CompletionSource) where T : Enum;
}
