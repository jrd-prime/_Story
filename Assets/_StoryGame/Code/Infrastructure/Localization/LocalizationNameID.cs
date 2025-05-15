namespace _StoryGame.Infrastructure.Localization
{
    public static class LocalizationNameID
    {
        private const string To = "to";
        private const string Main = "main";

        public const string Menu = "menu";
        public const string Start = "start";
        public const string Continue = "continue";
        public const string Settings = "settings";
        public const string Exit = "exit";
        public const string Audio = "audio";
        public const string Video = "video";
        public const string Pause = "pause";
        public const string Back = "back";
        public const string Inventory = "inventory";
        
        // Interact tips
        public const string TipCollect = "tip_collect";
        public const string TipOpen = "tip_open";
        public const string TipGather = "tip_gather";
        public const string TipLoot = "tip_loot";

        public static readonly string ToMainMenu = $"{To}-{Main}-{Menu}";
    }
}
