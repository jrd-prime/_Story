using _StoryGame.Gameplay.Character.Player.Impls;
using Zenject;

namespace _game.Scripts.Infrastructure.Installers.Game
{
    public sealed class SignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<MoveDirectionSignal>();
            // HSM
            // Container.DeclareSignal<ChangeGameStateSignalVo>();
            // UI Manager
            // Container.DeclareSignal<ShowViewSignalVo>();
            // Container.DeclareSignal<ShowPreviousViewSignalVo>();
            // Key
            // Container.DeclareSignal<EscapeKeySignal>();
            // Container.DeclareSignal<InventoryKeySignal>();
            // Container.DeclareSignal<InteractKeySignal>();
            // UI
            // Container.DeclareSignal<ShowInteractTipSignal>();
            // Container.DeclareSignal<HideInteractTipSignal>();
        }
    }
}
