using UnityEngine;

namespace _StoryGame.Core.Character.Player
{
    public sealed class PlayerService
    {
        public string Id => _model.Id;

        private readonly PlayerModel _model;

        public PlayerService(PlayerModel model) => _model = model;

        public void SetPosition(Vector3 position) => _model.Position = position;
    }
}
