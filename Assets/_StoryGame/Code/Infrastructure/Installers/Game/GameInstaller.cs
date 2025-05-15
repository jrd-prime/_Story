using System;
using _StoryGame.Core.Currency;
using _StoryGame.Core.Currency.Impls;
using _StoryGame.Core.Managers.Game.Impls;
using _StoryGame.Core.Managers.Game.Interfaces;
using _StoryGame.Core.Managers.HSM.Impls;
using _StoryGame.Gameplay.Managers.Impls;
using _StoryGame.Gameplay.Managers.Impls._Game._Scripts.Framework.Manager.JCamera;
using ModestTree;
using UnityEngine;
using Zenject;

namespace _StoryGame.Infrastructure.Installers.Game
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField] private CameraManager cameraManager;

        [SerializeField] private GameObject uiManager;

        // [SerializeField] private UIManager uiManagerPrefab;
        [SerializeField] private GameManager gameManagerPrefab;

        private GameObject _mainEmpty;

        public override void InstallBindings()
        {
            // Container.Bind<IUIManagerNew>().FromComponentInNewPrefab(uiManager).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HSM>().AsSingle().NonLazy();
            Log.Info("<color=cyan>GameInstaller</color>");


            Container.BindInterfacesAndSelfTo<FullScreenMovementViewModel>().AsSingle();
            _mainEmpty = GameObject.Find("--- MAIN");
            if (!_mainEmpty) throw new NullReferenceException("Main empty game object is not found. (--- MAIN)");

            Container.BindInterfacesTo<GameService>().AsSingle().NonLazy();
            Container.Bind<ICurrencyService>().To<CurrencyService>().AsSingle().NonLazy();
            InitializeManagers();
            InitializeUIModelsAndViewModels();
            InitializeViewStates();
        }

        private void InitializeManagers()
        {
            // RegisterComponent(cameraManager, "CameraManager");
            RegisterPrefabComponent(cameraManager, "CameraManager");
            RegisterPrefabComponent<GameManager>(gameManagerPrefab, "GameManager");
            // RegisterPrefabComponent<UIManager>(uiManagerPrefab, "UIManager");
        }

        private void InitializeViewStates()
        {
            // container.BindInterfacesAndSelfTo<NewMenuState>().AsSingle();
            // container.BindInterfacesAndSelfTo<NewGameplayState>().AsSingle();
            // container.BindInterfacesAndSelfTo<PauseState>().AsSingle();
        }

        private void InitializeUIModelsAndViewModels()
        {
            // Container.BindInterfacesAndSelfTo<MenuModel>().AsSingle();
            // Container.BindInterfacesAndSelfTo<MenuViewModel>().AsSingle();
            //
            // Container.BindInterfacesAndSelfTo<GameplayModel>().AsSingle();
            // Container.BindInterfacesAndSelfTo<GameplayViewModel>().AsSingle();
        }

        private void RegisterComponent<T>(T component, string componentName) where T : class
        {
            if (component == null)
                throw new NullReferenceException($"{componentName} is null in {gameObject.name}");

            Container.BindInterfacesAndSelfTo<T>()
                .FromInstance(component)
                .AsSingle();
        }

        private void RegisterPrefabComponent<T>(T prefabComponent, string componentName)
            where T : MonoBehaviour
        {
            if (prefabComponent == null)
                throw new NullReferenceException($"{componentName} is null in {gameObject.name}");

            Container.BindInterfacesAndSelfTo<T>()
                .FromComponentInNewPrefab(prefabComponent.gameObject) // Извлекаем GameObject из MonoBehaviour
                .AsSingle().OnInstantiated<T>((ctx, vacuumContainer) =>
                {
                    vacuumContainer.transform.parent = _mainEmpty.transform;
                });
        }
    }
}
