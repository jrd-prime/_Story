using _StoryGame.Core.Currency.Enums;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Currency
{
    [CreateAssetMenu(
        fileName = nameof(CoreItemData),
        menuName = SOPathConst.Currency + nameof(CoreItemData)
    )]
    public class CoreItemData : ACurrencyData
    {
        public override ECurrencyType Type { get; } = ECurrencyType.CoreItem;
    }
}
