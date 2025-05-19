using _StoryGame.Core.Managers.HSM.Interfaces;

namespace _StoryGame.Core.Managers.HSM.Impls.States.Gameplay
{
    public class GameplayState : BaseState
    {
        public override GameStateType StateType => GameStateType.Gameplay;

        public GameplayState(HSM hsm) : base(hsm)
        {
        }


        public override void Enter(IState previousState)
        {
            // UIManager.ShowView(new UIManagerViewDataVo(ViewRegistryType.Gameplay, ViewIDConst.Main));
        }

        public override void Exit(IState previousState)
        {
            // UIManager.HideAllViews();
        }
    }
}
