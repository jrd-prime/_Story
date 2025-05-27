using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(NoteCurrencyAData),
        menuName = SOPathConst.Currency + nameof(NoteCurrencyAData)
    )]
    public class NoteCurrencyAData : CurrencyASettings
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Note;
    }
}
