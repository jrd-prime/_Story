using System;
using System.Collections.Generic;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.SO.Currency;
using UnityEngine;

namespace _StoryGame.Data.Room
{
    [Serializable]
    public struct RoomLootVo
    {
        public CoreNoteData coreNoteData;
        public SpecialItemData hidden;
        public InspectableLootVo inspectableLoot;

        public void ShowPossibleLoot()
        {
            Debug.LogWarning("Possible Loot: ");
            Debug.LogWarning("Core Note: " + coreNoteData.LocalizationKey + " - " + coreNoteData.Type);
            Debug.LogWarning("Hidden: " + hidden.LocalizationKey + " - " + hidden.Type);
            Debug.LogWarning("Core Item: " + inspectableLoot.coreItem.coreItemData.LocalizationKey + " - " +
                             inspectableLoot.coreItem.coreItemData.Type);

            foreach (var note in inspectableLoot.notes.notes)
            {
                Debug.LogWarning("Note: " + note.LocalizationKey + " - " + note.Type);
            }

            Debug.LogWarning("Energy: " + inspectableLoot.energy.energy);
        }

        public InspectableLootVo GetLootObjects() => inspectableLoot;
    }
}
