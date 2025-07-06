using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Data.SO.Interactables;
using _StoryGame.Game.Loot;
using VContainer;

namespace _StoryGame.Infrastructure.Interact
{
    /// <summary>
    /// Легковесный (Flyweight) контейнер зависимостей для системы взаимодействий.
    /// Содержит общие сервисы, необходимые для работы интерактивных объектов.
    /// </summary>
    /// <remarks>
    /// Этот класс предназначен для уменьшения количества зависимостей в стратегиях и других компонентах,
    /// связанных с обработкой взаимодействий. Все зависимости разрешаются через DI-контейнер (VContainer).
    /// </remarks>
    public sealed class InteractSystemDepFlyweight
    {
        public readonly IJPublisher Publisher;
        public readonly IJLog Log;
        public readonly IL10nProvider L10n;
        public readonly InteractableSystemTipData InteractableSystemTipData;
        public readonly LootGenerator LootGenerator;


        public InteractSystemDepFlyweight(IObjectResolver resolver)
        {
            Publisher = resolver.Resolve<IJPublisher>();
            Log = resolver.Resolve<IJLog>();
            L10n = resolver.Resolve<IL10nProvider>();

            var settingsProvider = resolver.Resolve<ISettingsProvider>();
            InteractableSystemTipData = settingsProvider.GetSettings<InteractableSystemTipData>();

            LootGenerator = resolver.Resolve<LootGenerator>();
        }
    }
}
