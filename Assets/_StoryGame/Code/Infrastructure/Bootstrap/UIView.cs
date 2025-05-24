using System;
using _StoryGame.Game.UI.Impls;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public abstract class UIView<TController> : UIViewBase where TController : IUIViewModel
    {
        protected TController ViewModel;

        private void Start()
        {
            ResolveDependencies();

            InitElements();
            Subscribe();
        }

        public override void ShowBase()
        {
            Debug.Log("Show " + name);
            Root.style.display = DisplayStyle.Flex;
        }

        public override void HideBase()
        {
            Debug.Log("Hide " + name);
            Root.style.display = DisplayStyle.None;
        }

        private void ResolveDependencies()
        {
            if (Resolver == null)
                throw new NullReferenceException("Resolver is null in " + name);

            ViewModel = Resolver.Resolve<TController>();

            ResolveDependencies(Resolver);
        }

        protected virtual void ResolveDependencies(IObjectResolver resolver)
        {
        }

        protected abstract void InitElements();
        protected abstract void Subscribe();
    }
}
