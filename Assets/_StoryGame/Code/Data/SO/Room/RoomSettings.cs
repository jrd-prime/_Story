using System;
using System.Collections.Generic;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Currency;
using _StoryGame.Game.Interactables.Impls;
using UnityEngine;

namespace _StoryGame.Data.SO.Room
{
    [CreateAssetMenu(
        fileName = nameof(RoomSettings),
        menuName = SOPathConst.Settings + nameof(RoomSettings)
    )]
    public class RoomSettings : ASettingsBase
    {
        [SerializeField] private string roomId;
        [SerializeField] private RoomLootVo loot;
        public RoomLootVo Loot => loot;
        public string Id => roomId;
    }

    [Serializable]
    public struct RoomInteractablesVo
    {
        /// <summary>
        /// Кор объект, который содержит основные подсказки
        /// </summary>
        public MultiStage core;

        public List<Conditional> hidden;
        public List<Inspectable> inspectables;
    }

    [Serializable]
    public struct RoomLootVo
    {
        public CoreNoteData coreNoteData;
        public SpecialItemData hidden;
        public InspectableLootVo inspectableLoot;
    }

    [Serializable]
    public struct InspectableLootVo
    {
        public CoreItemVo coreItem;
        public NotesVo notes;
        public EnergyVo energy;
    }

    [Serializable]
    public struct CoreItemVo
    {
        public CoreItemData coreItemData;
    }

    [Serializable]
    public struct NotesVo
    {
        public List<NoteData> notes;
    }

    [Serializable]
    public struct EnergyVo
    {
        public int energy;
    }
}
