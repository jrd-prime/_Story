using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(SpecialCurrencyData),
        menuName = SOPathConst.Currency + nameof(SpecialCurrencyData)
    )]
    public class SpecialCurrencyData : CurrencySettings
    {
        public override string ConfigName { get; } = nameof(SpecialCurrencyData);
        public override ECurrencyType Type { get; } = ECurrencyType.Special;
    }
}
