using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.Loot
{
    public record LootData(string RoomId, string InteractableId, Sprite Icon, ACurrencyData Currency)
    {
        public string RoomId { get; } = RoomId;
        public string InteractableId { get; } = InteractableId;
        public Sprite Icon { get; } = Icon;
        public ACurrencyData Currency { get; } = Currency;
    }
}
