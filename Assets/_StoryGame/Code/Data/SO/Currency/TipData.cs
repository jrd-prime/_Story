using _StoryGame.Core.Currency.Enums;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Currency
{
    [CreateAssetMenu(
        fileName = nameof(TipData),
        menuName = SOPathConst.Currency + nameof(TipData)
    )]
    public class TipData : ACurrencyData
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Tip;
    }
}
