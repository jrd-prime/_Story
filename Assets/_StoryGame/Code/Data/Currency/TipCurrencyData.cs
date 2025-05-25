using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(TipCurrencyData),
        menuName = SOPathConst.Currency + nameof(TipCurrencyData)
    )]
    public class TipCurrencyData : CurrencySettings
    {
        public override string ConfigName { get; } = nameof(TipCurrencyData);
        public override ECurrencyType Type { get; } = ECurrencyType.Tip;
    }
}
