using _StoryGame.Core.Currency;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Currency
{
    [CreateAssetMenu(
        fileName = nameof(ThoughtData),
        menuName = SOPathConst.Currency + nameof(ThoughtData)
    )]
    public class ThoughtData : ACurrencyData
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Tip;
    }
}
