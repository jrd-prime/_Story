using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Currency.Interfaces;
using _StoryGame.Core.Providers.Assets;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Data.SO.Currency;
using _StoryGame.Data.SO.Main;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Core.Currency.Impls
{
    //TODO ref
    public sealed class CurrencyRegistry : ICurrencyRegistry
    {
        public string Description => "Currency Registry";
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///  Кэш иконок // iconId -> icon
        /// </summary>
        private readonly Dictionary<string, Sprite> _currencyIcons = new(); // iconId -> icon

        /// <summary>
        ///  Кэш данных валют // currencyId -> currency
        /// </summary>
        private readonly Dictionary<string, ICurrency> _currencyData = new(); // currencyId -> currency

        private readonly ISettingsProvider _settingsProvider;
        private readonly IAssetProvider _assetProvider;
        private readonly IJLog _log;

        public CurrencyRegistry(
            ISettingsProvider settingsProvider,
            IAssetProvider assetProvider,
            IJLog log
        ) => (_settingsProvider, _assetProvider, _log) = (settingsProvider, assetProvider, log);

        public async UniTask InitializeOnBoot()
        {
            var currenciesData = _settingsProvider.GetSettings<CurrenciesData>();

            if (!currenciesData)
                throw new Exception("CurrenciesData is null.");

            foreach (var currency in currenciesData.GetAll())
            {
                _log.Warn("Load icon for currency: " + currency.IconId + ". Currency id: " + currency.Id);

                if (currency is ThoughtData)
                    continue;

                var loadIcon = LoadIcon(currency.IconId);
                _currencyIcons.TryAdd(currency.IconId, loadIcon);
                _currencyData.TryAdd(currency.Id, currency);
            }

            _log.Info("CurrencyRegistry initialized with " + _currencyData.Count + " currencies.");


            IsInitialized = true;

            await UniTask.Yield();
        }

        public ICurrency Get(string uid, string currencyId)
        {
            if (_currencyData.TryGetValue(currencyId, out var currency))
                return currency;

            _log.Error("Not found currency: " + currencyId);
            return null;
        }

        public Sprite GetIcon(string currencyId)
        {
            if (_currencyIcons.TryGetValue(currencyId, out var icon))
                return icon;

            _log.Warn("Not found icon in cache for currency: " + currencyId + ". Try to load...");

            var iconId = GetIconId(currencyId);

            var loadIcon = LoadIcon(iconId);
            _currencyIcons.TryAdd(iconId, loadIcon);

            return loadIcon;
        }

        public int ConvertCurrency(string walletUid, string fromCurrencyId, string toCurrencyId, long amount) => 1;
        public float GetConversionRate(string fromCurrencyId, string toCurrencyId) => 1;
        public IReadOnlyList<ICurrency> GetAll() => _currencyData.Values.ToList();
        public IReadOnlyList<ICurrency> GetCurrenciesByType(ECurrencyType type) => null;
        public IReadOnlyList<ICurrency> GetCurrenciesByRarity(ECurrencyRarity rarity) => null;

        private string GetIconId(string currencyId)
        {
            if (_currencyData.TryGetValue(currencyId, out var currency))
                return currency.IconId;

            throw new Exception("Not found currency: " + currencyId);
        }

        private Sprite LoadIcon(string iconId)
        {
            try
            {
                return _assetProvider.LoadAsset<Sprite>("Icons/" + iconId + "_icon.png");
            }
            catch (Exception ex)
            {
                _log.Error($"Error loading icon with ID {iconId}: {ex.Message}");
                throw;
            }
        }
    }
}
