using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(SpecialCurrencyAData),
        menuName = SOPathConst.Currency + nameof(SpecialCurrencyAData)
    )]
    public class SpecialCurrencyAData : CurrencyASettings
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Special;
    }
}
