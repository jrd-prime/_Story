using _StoryGame.Core.Currency.Enums;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Currency
{
    [CreateAssetMenu(
        fileName = nameof(NoteData),
        menuName = SOPathConst.Currency + nameof(NoteData)
    )]
    public class NoteData : ANoteData
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Note;
    }
}
