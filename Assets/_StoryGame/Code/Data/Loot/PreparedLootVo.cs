using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.Loot
{
    public record PreparedLootVo(ACurrencyData Currency, int Amount, Sprite Icon, LootItemInfoVo Info)
    {
        public string RoomId => Info.RoomId;
        public string InteractableId => Info.InteractableId;
        public string LocalizedName => Info.LocalizedName;
        public int Amount { get; } = Amount;
    }
}
