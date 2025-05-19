using System;
using System.Collections.Generic;
using _StoryGame.Core.Managers.HSM.Impls.States;
using _StoryGame.Data.UI;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Gameplay.Interactables;
using _StoryGame.Gameplay.Movement;
using _StoryGame.Gameplay.UI.Interfaces;
using _StoryGame.Infrastructure.Tools;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class UIViewer : MonoBehaviour, IUIViewer
    {
        [SerializeField] private VisualTreeAsset popUpTreeAsset;

        private bool _isInitialized;
        private readonly Dictionary<GameStateType, TemplateContainer> _viewsCache = new();
        private VisualElement _viewerRoot;
        private VisualElement _layerBack;
        private VisualElement _layerMovement;
        private VisualElement _layerTop;
        private VisualElement _mainContainer;
        private VisualElement _movementArea;
        private VisualElement _ring;
        private IMovementHandler _movementHandler;

        private readonly CompositeDisposable _disposables = new();
        private FPSCounter _fpsCounter;
        private Label _fpsLabel;
        private VisualElement _topMainContainer;
        private VisualElement _layerPopUp;
        private VisualElement _popUpMainContainer;
        private VisualElement _popUpCenter;

        private TemplateContainer _popUpTemplate;
        private Label label;
        private Button btn;
        private ReactiveCommand _msgCommand;

        [Inject]
        private void Construct(IMovementHandler movementHandler, FPSCounter fpsCounter,
            ISubscriber<IUIViewerMessage> subscriber)
        {
            _movementHandler = movementHandler;
            _fpsCounter = fpsCounter;
            subscriber.Subscribe(OnMessage).AddTo(_disposables);
        }

        private void OnMessage(IUIViewerMessage message)
        {
            switch (message)
            {
                case ShowPopUpMessage msg:
                    ShowPopUp(msg.Id, msg.Command, msg.PositionType);
                    break;
                case ResetPopUpMessage msg:
                    ResetPopUp(msg.Id);
                    break;
            }
        }


        private async void Awake()
        {
            var document = GetComponent<UIDocument>();
            await document.WaitForReadyAsync();

            _viewerRoot = document.rootVisualElement;
            _viewerRoot.SetFullScreen();

            var safeZoneOffset = ScreenHelper.GetSafeZoneOffset(1600f, 720f);
            _viewerRoot.style.marginLeft = safeZoneOffset.x >= 16 ? safeZoneOffset.x : 16;
            _viewerRoot.style.marginTop = safeZoneOffset.y;

            _mainContainer = _viewerRoot.GetVisualElement<VisualElement>(UIConst.MainContainer, name);

            _layerBack = _mainContainer.GetVisualElement<VisualElement>("layer-0", name);
            _layerMovement = _mainContainer.GetVisualElement<VisualElement>("layer-1", name);
            _layerTop = _mainContainer.GetVisualElement<VisualElement>("layer-2", name);
            _layerPopUp = _mainContainer.GetVisualElement<VisualElement>("layer-popup", name);

            _topMainContainer = _layerTop.GetVisualElement<VisualElement>(UIConst.MainContainer, name);

            _popUpMainContainer = _layerPopUp.GetVisualElement<VisualElement>(UIConst.MainContainer, name);
            _popUpCenter = _popUpMainContainer.GetVisualElement<VisualElement>("center", name);

            _popUpTemplate = popUpTreeAsset.Instantiate();
            _popUpTemplate.pickingMode = PickingMode.Ignore;

            _layerBack.SetFullScreen();
            _layerMovement.SetFullScreen();
            _layerTop.SetFullScreen();


            _fpsLabel = _layerTop.GetVisualElement<Label>("fps", name);
            _fpsCounter.Fps.Subscribe(ShowFps).AddTo(_disposables);

            //TODO movement вынести

            _movementArea = _layerMovement.GetVisualElement<VisualElement>("movement-area", name);
            _ring = _layerMovement.GetVisualElement<VisualElement>("ring", name);

            if (_movementHandler == null)
                throw new NullReferenceException(nameof(_movementHandler));

            _movementHandler.IsTouchVisible.Subscribe(IsTouchPositionVisible).AddTo(_disposables);
            _movementHandler.RingPosition.Subscribe(SetRingPosition).AddTo(_disposables);

            _movementArea.RegisterCallback<PointerDownEvent>(_movementHandler.OnPointerDown);
            _movementArea.RegisterCallback<PointerMoveEvent>(_movementHandler.OnPointerMove);
            _movementArea.RegisterCallback<PointerUpEvent>(_movementHandler.OnPointerUp);
            _movementArea.RegisterCallback<PointerOutEvent>(_movementHandler.OnPointerCancel);
        }

        public void Initialize(Dictionary<GameStateType, UIViewBase> viewsCache)
        {
            foreach (var view in viewsCache)
                _viewsCache.TryAdd(view.Key, view.Value.Template);

            _isInitialized = true;
        }

        private void ShowFps(float value) => _fpsLabel.text = value.ToString();

        public void SwitchTo(GameStateType state)
        {
            if (!_isInitialized)
                throw new NullReferenceException("UIViewer is not initialized. " + nameof(SwitchTo));

            if (_viewsCache.TryGetValue(state, out var view))
            {
                _topMainContainer.Clear();
                _topMainContainer.Add(view);
                var viewMain = view.GetVisualElement<VisualElement>(UIConst.MainContainer, name);
                viewMain.style.display = DisplayStyle.Flex;
            }
        }

        private void ShowPopUp(string view, ReactiveCommand msgCommand, PositionType positionType = PositionType.Center)
        {
            switch (positionType)
            {
                case PositionType.Left:
                    break;
                case PositionType.Center:
                    _popUpCenter.Clear();
                    PreparePopUp(view, msgCommand);
                    _popUpCenter.Add(_popUpTemplate);
                    break;
                case PositionType.Right:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(positionType), positionType, null);
            }
        }

        private void ResetPopUp(string msgId)
        {
            btn.UnregisterCallback<ClickEvent>(OnPopUpBtnClick);
            _popUpCenter.Clear();
        }

        private void PreparePopUp(string view, ReactiveCommand msgCommand)
        {
            _msgCommand = msgCommand;
            label = _popUpTemplate.GetVisualElement<Label>("label", name);
            label.text = view;
            btn = _popUpTemplate.GetVisualElement<Button>("btn", name);

            btn.RegisterCallback<ClickEvent>(OnPopUpBtnClick);
        }

        private void OnPopUpBtnClick(ClickEvent _)
        {
            _msgCommand.Execute(Unit.Default);
            btn.UnregisterCallback<ClickEvent>(OnPopUpBtnClick);
            _popUpCenter.Clear();
        }


        private void SetRingPosition(Vector2 position)
        {
            _ring.style.left = position.x;
            _ring.style.top = position.y;
        }

        private void IsTouchPositionVisible(bool value) =>
            _ring.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;

        private void OnDestroy()
        {
            _isInitialized = false;
            _viewsCache.Clear();
        }
    }

    public enum PositionType
    {
        Left,
        Center,
        Right
    }
}
