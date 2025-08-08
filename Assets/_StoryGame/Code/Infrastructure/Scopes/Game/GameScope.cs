using System;
using _StoryGame.Core.HSM.Impls;
using _StoryGame.Core.WalletNew.Impls;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Game.Character.Player.Impls;
using _StoryGame.Game.Interact;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Inspectable.Providers;
using _StoryGame.Game.Interact.Inspectable.Systems;
using _StoryGame.Game.Interact.Passable.Providers;
using _StoryGame.Game.Interact.Passable.Systems;
using _StoryGame.Game.Interact.SortMbDelete;
using _StoryGame.Game.Interact.SortMbDelete.Conditional;
using _StoryGame.Game.Interact.SortMbDelete.Toggle;
using _StoryGame.Game.Interact.SortMbDelete.Use;
using _StoryGame.Game.Interact.Switchable.Systems;
using _StoryGame.Game.Interact.todecor;
using _StoryGame.Game.Interact.todecor.Impl.DeviceSystems;
using _StoryGame.Game.Loot;
using _StoryGame.Game.Managers;
using _StoryGame.Game.Managers._Game._Scripts.Framework.Manager.JCamera;
using _StoryGame.Game.Managers.Condition;
using _StoryGame.Game.Managers.Game;
using _StoryGame.Game.Managers.Interfaces;
using _StoryGame.Game.Managers.Room;
using _StoryGame.Game.Movement;
using _StoryGame.Game.UI.Impls.Viewer;
using _StoryGame.Game.UI.Impls.Views.Gameplay;
using _StoryGame.Game.UI.Impls.Views.Menu;
using _StoryGame.Game.UI.Impls.Views.WorldViews;
using _StoryGame.Infrastructure.Interact;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Scopes.Game
{
    public sealed class GameScope : LifetimeScope
    {
        [FormerlySerializedAs("playerPrefab")] [SerializeField]
        private PlayerView playerViewPrefab;

        [SerializeField] private Transform spawnPoint;

        [FormerlySerializedAs("prefab")] [SerializeField]
        private InteractablesTipUI interactablesTipUIPrefab;

        private GameObject _mainEmpty;

        protected override void Configure(IContainerBuilder builder)
        {
            // Debug.Log($"<color=cyan>{nameof(GameScope)}</color>");

            RegisterStateMachine(builder);

            // Поиск объекта --- MAIN
            _mainEmpty = GameObject.Find("--- MAIN");
            if (!_mainEmpty)
                throw new NullReferenceException("Main empty game object is not found. (--- MAIN)");

            // Регистрация сервисов
            builder.Register<GameService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<WalletService>(Lifetime.Singleton).As<IWalletService>();
            builder.Register<GameplayUIViewModel>(Lifetime.Singleton).As<IGameplayUIViewModel>();
            builder.Register<MenuUIViewModel>(Lifetime.Singleton).As<IMenuUIViewModel>();

            InitializeManagers(builder);
            InitializeUIModelsAndViewModels(builder);
            InitializeViewStates(builder);

            // var playerInstance = Instantiate(playerPrefab);
            var playerInstaller = new PlayerInstaller(builder, null, spawnPoint);
            if (!playerInstaller.Install())
                throw new Exception("PlayerInstaller is not installed.");


            builder.RegisterComponentInHierarchy<NewMovementHandler>().As<IMovementHandler>();


            builder.Register<MovementProcessor>(Lifetime.Singleton).AsSelf();
            builder.RegisterBuildCallback(resolver => resolver.Resolve<MovementProcessor>());

            builder.Register<InteractProcessor>(Lifetime.Singleton).AsSelf();
            builder.RegisterBuildCallback(resolver => resolver.Resolve<InteractProcessor>());

            RegisterInteractableSystems(builder);

            RegisterDialogSystems(builder);

            RegisterRooms(builder);

            if (!interactablesTipUIPrefab)
                throw new NullReferenceException("interactablesTipUIPrefab is null.");
            builder.RegisterComponentInNewPrefab<InteractablesTipUI>(interactablesTipUIPrefab, Lifetime.Transient);

            builder.Register<LootGenerator>(Lifetime.Singleton).AsSelf();

            builder.Register<ConditionChecker>(Lifetime.Singleton).AsSelf();
            builder.Register<ConditionRegistry>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<InteractSystemDepFlyweight>(Lifetime.Singleton).AsSelf();
            builder.Register<ConditionalStrategyProvider>(Lifetime.Singleton).AsSelf();
            builder.Register<InspectStrategyProvider>(Lifetime.Singleton).AsSelf();
            builder.Register<UseStrategyProvider>(Lifetime.Singleton).AsSelf();
            builder.Register<PassableStrategyProvider>(Lifetime.Singleton).AsSelf();
            builder.Register<ToggleStrategyProvider>(Lifetime.Singleton).AsSelf();
        }


        private void RegisterRooms(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<RoomsRegistry>().AsImplementedInterfaces();
            builder.Register<RoomsDispatcher>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private void RegisterDialogSystems(IContainerBuilder builder)
        {
            // builder.Register<InteractableDialogSystem>(Lifetime.Singleton).AsSelf();
        }

        private static void RegisterInteractableSystems(IContainerBuilder builder)
        {
            builder.Register<InspectSystem>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<UseSystem>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PassSystem>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ToggleSystem>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<SimpleSwitchSystem>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<DynamicOnConditionSwitchSystem>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        protected override void Awake()
        {
            base.Awake();

            var interactables =
                FindObjectsByType<AInteractableBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            // Debug.Log($"Interact on scene: {interactables.Length}");

            foreach (var interactable in interactables)
                Container.Inject(interactable);

            var deviceUI = FindObjectsByType<ADeviceUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var device in deviceUI)
                Container.Inject(device);
        }

        private void RegisterStateMachine(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<HSM>().AsSelf().As<IDisposable>();
        }


        private void InitializeManagers(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CameraManager>().As<ICameraManager>();
            builder.RegisterComponentInHierarchy<GameManager>().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<UIManager>().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<UIViewer>().AsImplementedInterfaces();
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
