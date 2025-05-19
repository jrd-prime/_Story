using _StoryGame.Core.Managers.HSM.Impls.States.Gameplay;
using _StoryGame.Core.Managers.HSM.Interfaces;

namespace _StoryGame.Core.Managers.HSM.Impls.States.Menu
{
    public sealed class MenuState : BaseState
    {
        public override GameStateType StateType => GameStateType.Menu;

        public MenuState(HSM hsm) : base(hsm)
        {
        }


        public override void Enter(IState previousState)
        {
            // Если приходим иг геймплея, то попадаем в паузу
            if (previousState is GameplayState)
            {
                // UIManager.ShowView(new UIManagerViewDataVo(ViewRegistryType.Menu, ViewIDConst.Pause));
                return;
            }

            // UIManager.ShowView(new UIManagerViewDataVo(ViewRegistryType.Menu, ViewIDConst.Main));
        }

        public override void Exit(IState previousState)
        {
            // Если пришли из геймплея в меню, то по возврату в геймплей
            if (previousState is GameplayState)
            {
                // Log.Info("previous state is gameplay");
            }
        }

        public override IState HandleTransition()
        {
            return null;
        }
    }
}
