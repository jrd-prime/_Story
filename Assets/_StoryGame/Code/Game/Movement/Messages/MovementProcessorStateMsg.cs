namespace _StoryGame.Game.Movement
{
    public record MovementProcessorStateMsg(EMoveState State) : IMovementProcessorMsg
    {
        public string Name => nameof(MovementProcessorStateMsg);
        public EMoveState State { get; } = State;
    }
}
