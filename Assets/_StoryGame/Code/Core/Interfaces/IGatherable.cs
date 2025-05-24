using UnityEngine;

namespace _StoryGame.Core.Interfaces
{
    public interface IGatherable
    {
        void Gather(Transform viewMuzzleTransform);
        void ResetSuctionBar();
        Vector3 Position { get; }
        // LootDataVo[] Loot { get; }
        string Id { get; }
        QuestEventIdType qEvtId { get; }
        void UpdateSuctionTime(float currentSuctionTime, float suctionTime);
    }

    public enum QuestEventIdType
    {
        KillResident,
        CollectMushroom,
        CollectGrass,
        CollectVacuum,
        CookMeat,
        Gather
    }
}
