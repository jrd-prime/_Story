using System;
using _StoryGame.Gameplay.UI;
using _StoryGame.Gameplay.UI.Impls;
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
            MainContainer.style.display = DisplayStyle.Flex;
        }

        public override void HideBase()
        {
            Debug.Log("Hide " + name);
            MainContainer.style.display = DisplayStyle.None;
        }

        private void ResolveDependencies()
        {
            if (Resolver == null)
                throw new NullReferenceException("Resolver is null in " + name);

            ViewModel = Resolver.Resolve<TController>() ??
                        throw new NullReferenceException("ViewModel is null in " + name);

            ResolveDependencies(Resolver);
        }

        protected virtual void ResolveDependencies(IObjectResolver resolver)
        {
        }

        protected abstract void InitElements();
        protected abstract void Subscribe();
    }
}
