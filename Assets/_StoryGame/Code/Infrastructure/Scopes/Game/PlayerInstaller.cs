using System;
using _StoryGame.Core.Animations.Interfaces;
using _StoryGame.Core.Character.Player;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Gameplay.Anima.Impls;
using _StoryGame.Gameplay.Character.Player.Impls;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Scopes.Game
{
    public sealed class PlayerInstaller
    {
        public PlayerInstaller(IContainerBuilder builder, Player playerInstance, Transform spawnPoint)
        {
            Debug.Log($"<color=cyan>{nameof(PlayerInstaller)}</color>");

            if (builder == null)
                throw new NullReferenceException("resolver is null.");

            if (!playerInstance)
                throw new NullReferenceException("playerInstance is null.");

            if (!spawnPoint)
                throw new NullReferenceException("spawnPoint is null.");

            playerInstance.transform.position = spawnPoint.position;

            builder.RegisterComponent(playerInstance).AsImplementedInterfaces();
            
            builder.Register<PlayerInteractor>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PlayerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PlayerModel>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // builder.Register<PlayerAnimationService>(Lifetime.Singleton).As<IPlayerAnimationService>();
        }
    }
}
