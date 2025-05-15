using _StoryGame.Core.Animations.Interfaces;
using _StoryGame.Core.Character.Player;
using _StoryGame.Gameplay.Anima.Impls;
using _StoryGame.Gameplay.Character.Player.Impls;
using UnityEngine;
using Zenject;

namespace _StoryGame.Infrastructure.Installers.Game
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private Player playerPrefab;
        [SerializeField] private Transform spawnPoint;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Player>().FromComponentInNewPrefab(playerPrefab).AsSingle()
                .OnInstantiated<Player>((ctx, player) => { player.transform.position = spawnPoint.position; })
                .NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerModel>().AsSingle().NonLazy();
            Container.Bind<IPlayerAnimationService>().To<PlayerAnimationService>().AsSingle().NonLazy();
        }
    }
}
