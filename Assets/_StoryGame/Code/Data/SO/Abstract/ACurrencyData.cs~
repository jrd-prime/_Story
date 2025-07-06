using System;
using _StoryGame.Core.Currency;
using _StoryGame.Core.Currency.Interfaces;
using UnityEngine;

namespace _StoryGame.Data.SO.Abstract
{
    /// <summary>
    ///  Базовый класс валюты
    /// </summary>
    public abstract class ACurrencyData : ASettingsBase, ICurrency
    {
        [SerializeField] private string id;
        [SerializeField] private string localizationKey;
        [SerializeField] private string descriptionKey;
        [SerializeField] private string iconId;
        [SerializeField] private ECurrencyRarity rarity;
        [SerializeField] private int maxStackSize;
        [SerializeField] private int amount = 1;

        public string Id => id;
        public string Name { get; }
        public string Description { get; }
        public string Icon { get; }

        public string LocalizationKey => localizationKey;
        public string DescriptionKey => descriptionKey;
        public string IconId => iconId;
        public ECurrencyRarity Rarity => rarity;
        public abstract ECurrencyType Type { get; }
        public int MaxStackSize => maxStackSize;
        public int Amount => amount;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("id is null or empty. " + name);
        }
#endif
    }
}
