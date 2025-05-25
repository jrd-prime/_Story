using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(CoreNoteCurrencyData),
        menuName = SOPathConst.Currency + nameof(CoreNoteCurrencyData)
    )]
    public class CoreNoteCurrencyData : CurrencySettings
    {
        public override string ConfigName { get; } = nameof(CoreNoteCurrencyData);
        public override ECurrencyType Type { get; } = ECurrencyType.CoreNote;
    }
}
