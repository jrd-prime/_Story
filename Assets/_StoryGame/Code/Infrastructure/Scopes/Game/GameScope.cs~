using System;
using _StoryGame.Core.Currency;
using _StoryGame.Core.Currency.Impls;
using _StoryGame.Core.Managers.Game.Impls;
using _StoryGame.Core.Managers.HSM.Impls;
using _StoryGame.Gameplay.Character.Player.Impls;
using _StoryGame.Gameplay.Managers.Impls;
using _StoryGame.Gameplay.Managers.Impls._Game._Scripts.Framework.Manager.JCamera;
using _StoryGame.Gameplay.Managers.Inerfaces;
using _StoryGame.Gameplay.UI;
using _StoryGame.Gameplay.UI.GameplayUI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Scopes.Game
{
    public sealed class GameScope : LifetimeScope
    {
        [SerializeField] private Player playerPrefab;

        [SerializeField] private Transform spawnPoint;

        // [SerializeField] private UIManager uiManagerPrefab; // Закомментировано, как в исходном коде
        private GameObject _mainEmpty;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"<color=cyan>{nameof(GameScope)}</color>");
            // Регистрация HSM как Singleton с немедленной активацией
            builder.RegisterEntryPoint<HSM>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();

            // Поиск объекта --- MAIN
            _mainEmpty = GameObject.Find("--- MAIN");
            if (!_mainEmpty)
                throw new NullReferenceException("Main empty game object is not found. (--- MAIN)");

            // Регистрация сервисов
            builder.Register<GameService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CurrencyService>(Lifetime.Singleton).As<ICurrencyService>();
            builder.Register<GameplayUIViewModel>(Lifetime.Singleton).As<IGameplayUIViewModel>();

            InitializeManagers(builder);
            InitializeUIModelsAndViewModels(builder);
            InitializeViewStates(builder);

            var playerInstance = Instantiate(playerPrefab);
            var playerInstaller = new PlayerInstaller(builder, playerInstance, spawnPoint);

            if (!playerInstaller.Install())
                throw new Exception("PlayerInstaller is not installed.");
        }


        private void InitializeManagers(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CameraManager>().As<ICameraManager>();
            builder.RegisterComponentInHierarchy<GameManager>().AsImplementedInterfaces();
        }

        private void InitializeViewStates(IContainerBuilder builder)
        {
            // builder.Register<NewMenuState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // builder.Register<NewGameplayState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // builder.Register<PauseState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        private void InitializeUIModelsAndViewModels(IContainerBuilder builder)
        {
            // builder.Register<MenuModel>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // builder.Register<MenuViewModel>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // builder.Register<GameplayModel>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // builder.Register<GameplayViewModel>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        private void RegisterComponent<T>(IContainerBuilder builder, T component, string componentName) where T : class
        {
            if (component == null)
                throw new NullReferenceException($"{componentName} is null in {gameObject.name}");

            builder.RegisterInstance(component).As<T>().AsImplementedInterfaces();
        }

        private void RegisterPrefabComponent<T>(IContainerBuilder builder, T prefabComponent, string componentName)
            where T : MonoBehaviour
        {
            if (prefabComponent == null)
                throw new NullReferenceException($"{componentName} is null in {gameObject.name}");

            builder.RegisterComponentInNewPrefab(prefabComponent, Lifetime.Singleton)
                .As<T>()
                .AsImplementedInterfaces();
        }
    }
}
