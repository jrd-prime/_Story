using System;
using System.Collections.Generic;
using System.Reflection;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Main;
using _StoryGame.Data.SO.Room;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Infrastructure.Settings
{
    public sealed class SettingsProvider : ISettingsProvider
    {
        public bool IsInitialized { get; private set; }
        public string Description => "Settings Provider";

        private readonly MainSettings _mainSettings;
        private readonly Dictionary<Type, ASettingsBase> _cache = new();

        private readonly Dictionary<string, RoomData> _roomsCache = new(); // <roomId, settings>

        public SettingsProvider(MainSettings mainSettings) => _mainSettings = mainSettings;

        public async UniTask InitializeOnBoot()
        {
            if (!_mainSettings)
                throw new Exception("MainSettings is null " + nameof(SettingsProvider));

            var fields =
                typeof(MainSettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (!typeof(ASettingsBase).IsAssignableFrom(field.FieldType))
                    continue;

                var settings = field.GetValue(_mainSettings) as ASettingsBase;

                if (settings == null)
                {
                    Debug.LogWarning($"Field {field.Name} in MainSettings is null, skipping.");
                    continue;
                }

                if (!_cache.TryAdd(settings.GetType(), settings))
                    throw new Exception($"Duplicate settings type {settings.GetType()} found in cache.");
            }

            AddRoomsToCache(GetSettings<MainRoomSettings>());

            IsInitialized = true;
            await UniTask.CompletedTask;
        }

        private void AddRoomsToCache(MainRoomSettings roomsSettings)
        {
            foreach (var room in roomsSettings.Rooms)
                _roomsCache.TryAdd(room.Id, room);
        }

        public T GetSettings<T>() where T : ASettingsBase
        {
            if (_cache.TryGetValue(typeof(T), out var settings))
                return (T)settings;

            throw new Exception($"Settings {typeof(T).Name} not found in cache.");
        }

        public RoomData GetRoomSettings(string roomId)
        {
            if (_roomsCache.TryGetValue(roomId, out var settings))
                return settings;

            throw new Exception($"Room {roomId} not found in room cache.");
        }
    }
}
