using _StoryGame.Core.HSM.Interfaces;

namespace _StoryGame.Core.HSM.Impls.States.RoomDraft
{
    public sealed class RoomDraftState : BaseState
    {
        public override EGameStateType StateType => EGameStateType.RoomDraft;

        public RoomDraftState(HSM hsm) : base(hsm)
        {
        }

        public override void Enter(IState previousState)
        {
        }

        public override void Exit(IState previousState)
        {
        }
    }
}
