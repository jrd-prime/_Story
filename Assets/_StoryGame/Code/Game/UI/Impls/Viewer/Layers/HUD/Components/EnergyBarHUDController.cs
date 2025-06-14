using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Game.Extensions;
using DG.Tweening;
using R3;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.HUD.Components
{
    public sealed class EnergyBarHUDController : IDisposable
    {
        private const string EnergyContId = "energy-cont";
        private const string EnergyLabId = "energy-label";
        private const string SliderContId = "slider-cont";
        private const string SliderId = "slider";

        private VisualElement _energyCont;
        private Label _energyLab;
        private VisualElement _sliderCont;
        private VisualElement _slider;

        private float _sliderHolderWidth;
        private int _maxEnergy = 0;
        private int _energy = 0;
        private bool _isSliderInitialized;
        private float _currentSliderWidth;

        private readonly IPlayer _player;
        private readonly CompositeDisposable _disposables = new();

        public EnergyBarHUDController(IPlayer player) => _player = player;

        public void Init(VisualElement mainContainer)
        {
            _energyCont = mainContainer.GetVisualElement<VisualElement>(EnergyContId, nameof(EnergyBarHUDController));
            _energyLab = _energyCont.GetVisualElement<Label>(EnergyLabId, nameof(EnergyBarHUDController));
            _sliderCont = _energyCont.GetVisualElement<VisualElement>(SliderContId, nameof(EnergyBarHUDController));
            _slider = _sliderCont.GetVisualElement<VisualElement>(SliderId, nameof(EnergyBarHUDController));

            _sliderCont.RegisterCallback<GeometryChangedEvent>(OnSliderGeometryChanged);

            _player.MaxEnergy
                .Subscribe(OnMaxEnergyChanged)
                .AddTo(_disposables);

            _player.Energy
                .Subscribe(OnEnergyChanged)
                .AddTo(_disposables);
        }

        private void OnSliderGeometryChanged(GeometryChangedEvent evt)
        {
            _sliderHolderWidth = _sliderCont.resolvedStyle.width;
            _isSliderInitialized = true;
            UpdateSlider();
            _sliderCont.UnregisterCallback<GeometryChangedEvent>(OnSliderGeometryChanged);
        }

        private void OnMaxEnergyChanged(int maxEnergy)
        {
            _maxEnergy = maxEnergy;
            UpdateSlider();
        }

        private void OnEnergyChanged(int energy)
        {
            _energy = energy;
            UpdateSlider();
        }

        private void UpdateSlider()
        {
            if (!_isSliderInitialized || _maxEnergy == 0)
            {
                _energyLab.text = "0 / 0";
                _currentSliderWidth = _sliderHolderWidth;
                _slider.style.width = new Length(100, LengthUnit.Percent);
                return;
            }

            DOTween.To(
                () => _currentSliderWidth,
                x =>
                {
                    _currentSliderWidth = x;
                    _slider.style.width = new Length(_currentSliderWidth, LengthUnit.Pixel);
                },
                CalcTargetWidth(),
                1.3f
            ).SetEase(Ease.Linear);

            _energyLab.text = $"{_energy} / {_maxEnergy}";
        }

        private float CalcTargetWidth() => (float)_energy / _maxEnergy * _sliderHolderWidth;

        public void Dispose() => _disposables.Dispose();
    }
}
