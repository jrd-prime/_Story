using _StoryGame.Core.Currency;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Currency
{
    [CreateAssetMenu(
        fileName = nameof(EnergyData),
        menuName = SOPathConst.Currency + nameof(EnergyData)
    )]
    public class EnergyData : ACurrencyData
    {
        public override ECurrencyType Type { get; } = ECurrencyType.Energy;
    }
}
