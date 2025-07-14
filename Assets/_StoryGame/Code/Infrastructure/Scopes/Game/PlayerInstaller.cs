using _StoryGame.Core.Character.Player;
using _StoryGame.Game.Character.Player.Impls;
using _StoryGame.Game.UI.Impls.Views.WorldViews;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Scopes.Game
{
    public sealed class PlayerInstaller
    {
        private readonly IContainerBuilder _builder;
        private readonly PlayerView _instance;
        private readonly Transform _point;

        public PlayerInstaller(IContainerBuilder builder, PlayerView instance, Transform spawnPoint) =>
            (_builder, _instance, _point) = (builder, instance, spawnPoint);

        public bool Install()
        {
            //  Debug.Log($"<color=cyan>{nameof(PlayerInstaller)}</color>");

            if (_builder == null
                // || !_instance
                || !_point)
                return false;

            // _instance.transform.position = _point.position;

            // _builder.RegisterComponent(_instance).AsImplementedInterfaces();

            _builder.RegisterComponentInHierarchy<PlayerView>().AsSelf().AsImplementedInterfaces();
            _builder.RegisterComponentInHierarchy<PlayerOverHeadUI>();

            _builder.Register<PlayerInteractor>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            _builder.Register<PlayerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            _builder.Register<PlayerModel>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // builder.Register<PlayerAnimationService>(Lifetime.Singleton).As<IPlayerAnimationService>();

            return true;
        }
    }
}
