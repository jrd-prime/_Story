using _StoryGame.Core.Currency;
using UnityEngine;

namespace _StoryGame.Data.Currency
{
    [CreateAssetMenu(
        fileName = nameof(NoteCurrencyData),
        menuName = SOPathConst.Currency + nameof(NoteCurrencyData)
    )]
    public class NoteCurrencyData : CurrencySettings
    {
        public override string ConfigName { get; } = nameof(NoteCurrencyData);
        public override ECurrencyType Type { get; } = ECurrencyType.Note;
    }
}
