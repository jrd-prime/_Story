using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(EnergyCurrencyData),
        menuName = SOPathConst.Currency + nameof(EnergyCurrencyData)
    )]
    public class EnergyCurrencyData : CurrencySettings
    {
        public override string ConfigName { get; } = nameof(EnergyCurrencyData);
        public override ECurrencyType Type { get; } = ECurrencyType.Energy;
    }
}
