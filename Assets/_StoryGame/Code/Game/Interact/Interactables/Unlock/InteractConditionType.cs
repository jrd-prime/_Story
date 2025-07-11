namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public enum InteractConditionType
    {
        NotSet = -1,
        HasElectricity = 1000,
        ServModuleHasPower = 1,
        ModulePersistentClosed = 2,
        MechWaterSupplyOff = 3
    }
}
