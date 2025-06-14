using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Currency.Interfaces;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Data.Const;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.UI.Abstract;
using _StoryGame.Game.UI.Impls.Viewer.Layers.HUD.Components;
using _StoryGame.Infrastructure.Tools;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.HUD
{
    public sealed class HUDLayerHandler : AUIViewerHandlerBase, IUIViewerLayerHandler
    {
        private const string FpsLabelId = "fps";

        private FPSCounter _fpsCounter;
        private Label _fpsLabel;
        private VisualElement _currentViewMainContainer = null;

        private InventoryHUDController _inventoryHUDController;
        private EnergyBarHUDController _energyBarHUDController;
        private IWallet _tempWallet;
        private VisualTreeAsset _invCellTemplate;

        public HUDLayerHandler(IObjectResolver resolver, VisualElement layerBack) : base(resolver, layerBack)
        {
        }

        protected override void PreInitialize()
        {
            _tempWallet = GameManager.TempWallet;
            _invCellTemplate = UISettings.inventoryHUDCellTemplate;
        }

        protected override void ResolveDependencies(IObjectResolver resolver)
        {
            var currencyRegistry = resolver.Resolve<ICurrencyRegistry>();
            _inventoryHUDController = new InventoryHUDController(currencyRegistry).AddTo(Disposables);

            var player = resolver.Resolve<IPlayer>();
            _energyBarHUDController = new EnergyBarHUDController(player).AddTo(Disposables);

            _fpsCounter = resolver.Resolve<FPSCounter>();
        }

        protected override void InitElements()
        {
            _fpsLabel = GetElement<Label>(FpsLabelId);
        }

        protected override void Subscribe()
        {
            Debug.Log("HUDLayerHandler.Subscribe");

            _tempWallet.OnCurrencyChanged
                .Subscribe(_inventoryHUDController.OnCurrencyChanged)
                .AddTo(Disposables);

            _fpsCounter.Fps
                .Subscribe(ShowFps)
                .AddTo(Disposables);
        }


        protected override void Unsubscribe()
        {
        }

        private void ShowFps(float value)
        {
            UniTask.Post(
                () => _fpsLabel.text = value.ToString("F1")
            );
        }

        // TODO интересное переключение с анимациями
        public void SwitchViewTo(TemplateContainer value)
        {
            if (_currentViewMainContainer != null)
                _currentViewMainContainer.style.display = DisplayStyle.None;

            MainContainer.Clear();
            MainContainer.Add(value);
            var newView = value.GetVisualElement<VisualElement>(UIConst.MainContainer, nameof(HUDLayerHandler));
            newView.style.display = DisplayStyle.Flex;
            _currentViewMainContainer = newView;

            _inventoryHUDController.Init(MainContainer, _invCellTemplate);
            _energyBarHUDController.Init(MainContainer);
        }
    }
}
