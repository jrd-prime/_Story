using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Extensions;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Data.SO.Main;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace _StoryGame.Infrastructure.Localization
{
    public sealed class L10NProvider : IL10nProvider
    {
        public bool IsInitialized { get; private set; }
        public string Description => "Unity Localization Provider";

        private const string WordsTable = "Words";
        private const string SmallPhraseTable = "SmallPhrase";
        private const string SimpleNoteTable = "SimpleNote";
        private const string CoreNoteTable = "CoreNote";

        private StringTable _wordsTable;
        private StringTable _smallPhraseTable;
        private StringTable _simpleNoteTable;
        private StringTable _coreNoteTable;

        private readonly ISettingsProvider _settingsProvider;
        private readonly IJLog _log;

        public L10NProvider(ISettingsProvider settingsProvider, IJLog log)
        {
            _settingsProvider = settingsProvider;
            _log = log;
        }

        public async UniTask InitializeOnBoot()
        {
            var settings = _settingsProvider.GetSettings<JLocalizationSettings>();

            if (settings == null)
                throw new NullReferenceException("Localization settings is null.");

            var localeCode = settings.DefaultLanguage switch
            {
                Language.English => "en",
                Language.Russian => "ru",
                _ => "en"
            };

            var locale = LocalizationSettings.AvailableLocales.Locales
                .Find(l => l.Identifier.Code == localeCode);

            if (locale == null)
                throw new Exception($"Locale not found for code: {localeCode}");

            await LocalizationSettings.InitializationOperation.Task;

            LocalizationSettings.SelectedLocale = locale;

            _wordsTable = await InitTableAsync(WordsTable);
            _smallPhraseTable = await InitTableAsync(SmallPhraseTable);
            _coreNoteTable = await InitTableAsync(CoreNoteTable);
            _simpleNoteTable = await InitTableAsync(SimpleNoteTable);

            IsInitialized = true;
        }

        private static async UniTask<StringTable> InitTableAsync(string tableId)
        {
            var table = await LocalizationSettings.StringDatabase.GetTableAsync(tableId);

            if (!table)
                throw new Exception($"Localization table \"{tableId}\" not found.");

            return table;
        }

        public string Localize(string key, ETable tableType, ETextTransform transform = ETextTransform.None)
        {
            if (!IsInitialized)
                throw new Exception("L10n is not initialized.");

            var table = GetTableByType(tableType);
            var entry = table.GetEntry(key);

            string value;
            if (entry == null)
            {
                _log.Error($"Localization key '{key}' not found.");
                value = "Not localized";
            }
            else value = entry.GetLocalizedString();

            return TransformWord(value, transform);
        }

        private StringTable GetTableByType(ETable tableType) =>
            tableType switch
            {
                ETable.SimpleNote => _simpleNoteTable,
                ETable.CoreNote => _coreNoteTable,
                ETable.SmallPhrase => _smallPhraseTable,
                ETable.Words => _wordsTable,
                _ => throw new ArgumentOutOfRangeException(nameof(tableType), tableType, null)
            };

        private static string TransformWord(string value, ETextTransform transform) =>
            transform switch
            {
                ETextTransform.None => value,
                ETextTransform.Capitalize => value.Capitalize(),
                ETextTransform.Low => value.ToLower(),
                ETextTransform.Upper => value.ToUpper(),
                _ => throw new ArgumentOutOfRangeException(nameof(transform), transform, null)
            };
    }
}
