using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(CoreNoteCurrencyAData),
        menuName = SOPathConst.Currency + nameof(CoreNoteCurrencyAData)
    )]
    public class CoreNoteCurrencyAData : CurrencyASettings
    {
        public override ECurrencyType Type { get; } = ECurrencyType.CoreNote;
    }
}
