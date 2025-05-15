namespace _StoryGame.Core.Character.Common.Interfaces
{
    public interface ICharacter : IMovable
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        object Animator { get; }
        int Health { get; }
        int MaxHealth { get; }
        ICharacterInteractor GetInteractor();
        CharacterState State { get; }
        void SetState(CharacterState state);
    }

    public enum CharacterState
    {
        Idle,
        Moving,
        Interacting
    }
}
