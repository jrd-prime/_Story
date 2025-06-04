using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Game.Extensions;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.HUD.Components
{
    public sealed class EnergyBarHUDController
    {
        private VisualElement _energyContainer;
        private Label _energyLabel;
        private readonly IPlayer _player;

        public EnergyBarHUDController(IPlayer player)
        {
            _player = player;
        }

        public void Init(VisualElement mainContainer)
        {
            _energyContainer =
                mainContainer.GetVisualElement<VisualElement>("energy-cont", nameof(EnergyBarHUDController));
            _energyLabel = _energyContainer.GetVisualElement<Label>("energy-label", nameof(EnergyBarHUDController));


            _energyLabel.text = _player.Energy.ToString();
        }
    }
}
