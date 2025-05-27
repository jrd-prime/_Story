using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(EnergyCurrencyAData),
        menuName = SOPathConst.Currency + nameof(EnergyCurrencyAData)
    )]
    public class EnergyCurrencyAData : CurrencyASettings
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Energy;
    }
}
