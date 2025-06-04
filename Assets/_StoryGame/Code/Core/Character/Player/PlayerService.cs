using _StoryGame.Core.Extensions;
using UnityEngine;

namespace _StoryGame.Core.Character.Player
{
    public sealed class PlayerService
    {
        public string Id => _model.Id;
        public int MaxEnergy { get; } = 10;

        private readonly PlayerModel _model;

        public PlayerService(PlayerModel model) => _model = model;

        public void SetPosition(Vector3 position)
        {
            _model.CheckOnNull(nameof(PlayerService));
            _model.Position = position;
        }
    }
}
