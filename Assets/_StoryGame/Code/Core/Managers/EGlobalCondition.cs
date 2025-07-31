namespace _StoryGame.Core.Managers
{
    public enum EGlobalCondition
    {
        NotSet = 0,
        HasElectricity = 1000,
        ServModuleHasPower = 1,
        ModulePersistentClosed = 2,

        /// <summary>
        /// Водоснабжение включено для мех. модуля
        /// </summary>
        MechWaterSupplySwitchedOn = 3,
        
        
        ServerElectricitySwitchedOn = 4
    }
}
