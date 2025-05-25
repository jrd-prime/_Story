using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(CoreItemCurrencyData),
        menuName = SOPathConst.Currency + nameof(CoreItemCurrencyData)
    )]
    public class CoreItemCurrencyData : CurrencySettings
    {
        public override string ConfigName { get; } = nameof(CoreItemCurrencyData);
        public override ECurrencyType Type { get; } = ECurrencyType.CoreItem;
    }
}
