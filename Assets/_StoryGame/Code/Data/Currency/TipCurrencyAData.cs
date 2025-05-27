using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(TipCurrencyAData),
        menuName = SOPathConst.Currency + nameof(TipCurrencyAData)
    )]
    public class TipCurrencyAData : CurrencyASettings
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Tip;
    }
}
