using _StoryGame.Core.Currency.Enums;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Currency
{
    [CreateAssetMenu(
        fileName = nameof(CoreNoteData),
        menuName = SOPathConst.Currency + nameof(CoreNoteData)
    )]
    public class CoreNoteData : ACurrencyData
    {
        public override ECurrencyType Type { get; } = ECurrencyType.CoreNote;
    }
}
