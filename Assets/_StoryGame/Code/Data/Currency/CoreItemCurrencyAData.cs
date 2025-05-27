using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(CoreItemCurrencyAData),
        menuName = SOPathConst.Currency + nameof(CoreItemCurrencyAData)
    )]
    public class CoreItemCurrencyAData : CurrencyASettings
    {
        public override ECurrencyType Type { get; } = ECurrencyType.CoreItem;
    }
}
