namespace _StoryGame.Data.Const
{
    public static class SOPathConst
    {
        // Names
        private const string MainMenu = ProjectConstant.AppName + "/";
        private const string Config = "Settings/";
        private const string Main = "Main/";
        private const string UI = "UI/";
        private const string Character = "character/";


        // Paths
        public const string Settings = MainMenu + Config;
        public const string MainSettings = Settings + Main;
        public const string CharacterPath = MainMenu + Config + Character;
        public const string UIPath = MainMenu + Config + UI;
        public const string UISettings = MainSettings + UI;
        public const string Currency = Settings + "Currency/";
        public const string Interactables = Settings + "Interactables/";
    }
}
