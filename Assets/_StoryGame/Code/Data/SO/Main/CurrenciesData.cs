using System.Collections.Generic;
using _StoryGame.Core.Currency.Interfaces;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Currency;
using UnityEngine;

namespace _StoryGame.Data.SO.Main
{
    [CreateAssetMenu(
        fileName = nameof(CurrenciesData),
        menuName = SOPathConst.MainSettings + nameof(CurrenciesData)
    )]
    public class CurrenciesData : ASettingsBase
    {
        public List<CoreItemData> coreItems;
        public List<CoreNoteData> coreNotes;
        public List<NoteData> notes;
        public List<SpecialItemData> specialItems;
        public List<TipData> tips;
        public EnergyData energy;

        public IEnumerable<ICurrency> GetAll()
        {
            var list = new List<ICurrency>();
            list.AddRange(coreItems);
            list.AddRange(coreNotes);
            list.AddRange(notes);
            list.AddRange(specialItems);
            list.AddRange(tips);
            list.Add(energy);
            return list;
        }
    }
}
