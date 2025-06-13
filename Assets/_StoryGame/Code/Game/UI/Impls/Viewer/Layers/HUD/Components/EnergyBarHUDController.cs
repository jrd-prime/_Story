using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Game.Extensions;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.HUD.Components
{
    public sealed class EnergyBarHUDController : IDisposable
    {
        private VisualElement _energyContainer;
        private Label _energyLabel;
        private readonly IPlayer _player;
        private readonly CompositeDisposable _disposables = new();
        private VisualElement _sliderHolder;
        private float _sliderHolderWidth;
        private int _maxEnergy;
        private bool _isSliderInitialized;

        public EnergyBarHUDController(IPlayer player)
        {
            _player = player;
        }

        public void Init(VisualElement mainContainer)
        {
            _energyContainer =
                mainContainer.GetVisualElement<VisualElement>("energy-cont", nameof(EnergyBarHUDController));
            _energyLabel = _energyContainer.GetVisualElement<Label>("energy-label", nameof(EnergyBarHUDController));
            _sliderHolder =
                _energyContainer.GetVisualElement<VisualElement>("slider-holder", nameof(EnergyBarHUDController));

            _sliderHolder.RegisterCallback<GeometryChangedEvent>(OnSliderGeometryChanged);

            _player.MaxEnergy.Subscribe(OnMaxEnergyChanged).AddTo(_disposables);
            _player.Energy.Subscribe(OnEnergyChanged).AddTo(_disposables);
        }

        private void OnSliderGeometryChanged(GeometryChangedEvent evt)
        {
            Debug.Log(_sliderHolder.resolvedStyle.width);
            _sliderHolderWidth = _sliderHolder.resolvedStyle.width;
            _isSliderInitialized = true;
            _sliderHolder.UnregisterCallback<GeometryChangedEvent>(OnSliderGeometryChanged);
        }

        private void OnMaxEnergyChanged(int maxEnergy)
        {
            _maxEnergy = maxEnergy;
            if (_isSliderInitialized)
            {
                UpdateSlider();
            }
        }

        private void UpdateSlider()
        {
        }

        private void OnEnergyChanged(int energy)
        {
            _energyLabel.text = energy.ToString();
        }

        public void Dispose() => _disposables.Dispose();
    }
}
