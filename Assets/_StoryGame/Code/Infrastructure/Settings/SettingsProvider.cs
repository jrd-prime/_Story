using System;
using System.Collections.Generic;
using System.Reflection;
using _StoryGame.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Infrastructure.Settings
{
    public sealed class SettingsProvider : ISettingsProvider
    {
        public bool IsInitialized { get; private set; }
        public string Description => "Settings Provider";

        private MainSettings _mainSettings;
        private readonly Dictionary<Type, object> _cache = new();

        public SettingsProvider(MainSettings mainSettings) => _mainSettings = mainSettings;

        public async UniTask InitializeOnBoot()
        {
            if (!_mainSettings)
                throw new Exception("MainSettings is null " + nameof(SettingsProvider));

            await AddSettingsToCacheAsync();

            IsInitialized = true;
        }

        private async UniTask AddSettingsToCacheAsync()
        {
            var fields =
                typeof(MainSettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (!typeof(SettingsBase).IsAssignableFrom(field.FieldType))
                    continue;

                var settings = field.GetValue(_mainSettings) as SettingsBase;

                if (settings == null)
                {
                    Debug.LogWarning($"Field {field.Name} in MainSettings is null, skipping.");
                    continue;
                }

                if (!_cache.TryAdd(settings.GetType(), settings))
                    throw new Exception($"Duplicate settings type {settings.GetType()} found in cache.");
            }

            // Log.Info($"Settings added to cache: {_cache.Count}");
            await UniTask.CompletedTask;
        }

        public T GetSettings<T>() where T : SettingsBase
        {
            if (!_cache.TryGetValue(typeof(T), out var settings))
                throw new Exception($"Settings {typeof(T).Name} not found in cache.");

            return (T)settings;
        }
    }
}
