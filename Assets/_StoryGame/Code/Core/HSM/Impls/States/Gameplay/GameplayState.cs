using _StoryGame.Core.HSM.Interfaces;

namespace _StoryGame.Core.HSM.Impls.States.Gameplay
{
    public class GameplayState : BaseState
    {
        public override EGameStateType StateType => EGameStateType.Gameplay;

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
