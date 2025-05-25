using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    public abstract class CurrencySettings : SettingsBase, ICurrency
    {
        [SerializeField] private string id;
        [SerializeField] private string localizationKey;
        [SerializeField] private string descriptionKey;
        [SerializeField] private string iconId;
        [SerializeField] private ECurrencyRarity rarity;
        [SerializeField] private int maxStackSize;

        public string Id => id;
        public string LocalizationKey => localizationKey;
        public string DescriptionKey => descriptionKey;
        public string IconId => iconId;
        public ECurrencyRarity Rarity => rarity;
        public abstract ECurrencyType Type { get; }
        public int MaxStackSize => maxStackSize;
    }
}
