namespace _StoryGame.Game.Room.Abstract
{
    public enum EExit
    {
        NotSet = -1,

        // B1
        B1SurfaceAccess = 0,

        // B2 exits
        B2CorridorMain = 1,
        B2CorridorLiving = 2,
        B2Server = 3,
        B2Med = 4,
        B2Hab5Player = 5,
        B2Relax = 6,
        B2Nutrition = 7,
        B2Hyg = 8,
        B2Hab1 = 9,
        B2Hab2 = 10,
        B2Hab3 = 11,
        B2Hab4 = 12,
        B2Hab6 = 13,

        // B3 exits
        B3CorridorMain = 14,
        B3Mech = 15,
        B3Clim = 16,
        B3Vault = 17,
        B3Ware = 18,
        B3Water = 19,
        B3Rec = 20,

        // Additional 
        B1B2Ladder = 21,
        B2B3Ladder = 22,
        B2B3Hatch = 23,
    }
}
