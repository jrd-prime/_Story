using _StoryGame.Core.Currency.Enums;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Currency
{
    [CreateAssetMenu(
        fileName = nameof(SpecialItemData),
        menuName = SOPathConst.Currency + nameof(SpecialItemData)
    )]
    public class SpecialItemData : ACurrencyData
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Special;
    }
}
