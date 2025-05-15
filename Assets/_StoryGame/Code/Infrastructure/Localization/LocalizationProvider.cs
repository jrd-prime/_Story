using System;
using System.Collections.Generic;
using _StoryGame.Data;
using _StoryGame.Infrastructure.Settings;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace _StoryGame.Infrastructure.Localization
{
    public sealed class LocalizationProvider : ILocalizationProvider
    {
        public bool IsInitialized { get; private set; }
        public string Description => "Localization Provider";

        private string _defaultLanguage;
        private ISettingsProvider _settingsProvider;

        private readonly Dictionary<string, string> _localisationCache = new();

        private readonly Dictionary<Language, string> _languages = new()
        {
            { Language.English, "en" },
            { Language.Russian, "ru" }
        };

        [Inject]
        private void Construct(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public async UniTask InitializeOnBoot()
        {
            if (_settingsProvider == null)
                throw new NullReferenceException("Settings provider is null. + " + nameof(LocalizationProvider));
            var languageSettings = _settingsProvider.GetSettings<LocalizationSettings>();

            _defaultLanguage = _languages[languageSettings.DefaultLanguage];

            try
            {
                await LoadLocalizationDataAsync(_defaultLanguage);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load localization data: {e.Message}");
            }

            IsInitialized = true;
            Debug.Log("Localization system initialization completed. Default language: " + _defaultLanguage);
        }

        public string Localize(string key, WordTransform wordTransform = WordTransform.None)
        {
            if (!_localisationCache.TryGetValue(key, out var value))
                throw new KeyNotFoundException($"Localization key '{key}' not found.");

            return wordTransform switch
            {
                WordTransform.None => value,
                WordTransform.Capitalize =>  "CAPITALIZE NOT IMPLEMENTED", //; value.Capitalize(),
                WordTransform.Low => value.ToLower(),
                WordTransform.Upper => value.ToUpper(),
                _ => throw new ArgumentOutOfRangeException(nameof(wordTransform), wordTransform, null)
            };
        }

        private async UniTask LoadLocalizationDataAsync(string address)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(address);
                var textAsset = await handle.Task;

                if (textAsset)
                {
                    var jsonContent = textAsset.text;
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                    foreach (var entry in data) _localisationCache.TryAdd(entry.Key, entry.Value);
                }
                else Debug.LogError($"Localization file not found at address: {address}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load localization data: {e.Message}");
            }
        }
    }
}
