namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public enum EGlobalInteractCondition
    {
        NotSet = -1,
        HasElectricity = 1000,
        ServModuleHasPower = 1,
        ModulePersistentClosed = 2,

        /// <summary>
        /// Водоснабжение включено для мех. модуля
        /// </summary>
        MechWaterSupplySwitchedOn = 3
    }
}
