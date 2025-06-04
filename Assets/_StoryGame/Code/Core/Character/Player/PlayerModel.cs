using UnityEngine;

namespace _StoryGame.Core.Character.Player
{
    public sealed class PlayerModel
    {
        public string Id => "player_id";
        public int Level { get; set; }
        public int Health { get; set; }
        public Vector3 Position { get; set; }
    }
}
