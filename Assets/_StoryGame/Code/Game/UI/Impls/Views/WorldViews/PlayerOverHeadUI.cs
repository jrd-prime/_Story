using System;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.UI.Const;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data.UI;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.Interactables.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Views.WorldViews
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(UIDocument))]
    public sealed class PlayerOverHeadUI : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private int panelWidth = 1600;
        [SerializeField] private int panelHeight = 720;
        [SerializeField] private float panelScale = 1f;
        [SerializeField] private float pixelsPerUnit = 500f;
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;
        [SerializeField] private RenderTexture renderTextureAsset;

        private const float ProgressBarShowHideDuration = 0.5f;
        private const float ProgressBarDefaultWidth = 0f;

        private UIDocument _uiDocument;
        private VisualElement _root;
        private VisualElement _progressCont;

        private float _progressBarContWidth;
        private readonly CompositeDisposable _disposables = new();
        private VisualElement _progressBarCont;
        private Label _actionLab;
        private VisualElement _thoughtBubbleCont;
        private Label _thoughtLab;
        private VisualElement _mainCont;

        private Tween _currentThoughtBubbleTween;
        private Tween _currentProgressBarTween;
        private UniTaskCompletionSource _thoughtBubbleTaskSource;
        private UniTaskCompletionSource _progressBarTaskSource;

        [Inject]
        private void Construct(ISubscriber<IPlayerOverHeadUIMsg> subscriber) =>
            subscriber.Subscribe(OnMessage).AddTo(_disposables);

        private void Start()
        {
            _uiDocument = GetComponent<UIDocument>();
            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            var panelSettings = Instantiate(panelSettingsAsset);
            panelSettings.name = "PlayerOverheadTipPanelSettings";

            var initializer = new TipInitializer();
            initializer.Init(new WorldTipData(_uiDocument, visualTreeAsset, transform, meshFilter, meshRenderer,
                renderTextureAsset, panelSettings, panelWidth, panelHeight, panelScale, pixelsPerUnit));

            _root = _uiDocument.rootVisualElement;
            _mainCont = _root.GetVElement<VisualElement>(OverHeadUIConst.MainCont, name);

            InitProgressBarElements();
            InitThoughtBubbleElements();

            _progressBarCont.RegisterCallback<GeometryChangedEvent>(OnGeometryLoaded);
        }

        private void InitProgressBarElements()
        {
            _progressBarCont = _mainCont.GetVElement<VisualElement>(OverHeadUIConst.ProgressBarCont, name);
            _progressCont = _progressBarCont.GetVElement<VisualElement>(OverHeadUIConst.Progress, name);
            _actionLab = _progressBarCont.GetVElement<Label>(OverHeadUIConst.ActionLab, name);
        }

        private void InitThoughtBubbleElements()
        {
            _thoughtBubbleCont = _mainCont.GetVElement<VisualElement>(OverHeadUIConst.ThoughtBubbleCont, name);
            _thoughtLab = _thoughtBubbleCont.GetVElement<Label>(OverHeadUIConst.ThoughtLab, name);
        }

        private void LateUpdate()
        {
            if (!mainCamera)
                return;

            transform.LookAt(mainCamera.transform.position);
            transform.Rotate(0, 180f, 0);
        }

        private void OnMessage(IPlayerOverHeadUIMsg msg)
        {
            CancelCurrentAnimations();

            switch (msg)
            {
                case DisplayProgressBarMsg message:
                    DisplayProgressBar(message).Forget();
                    break;
                case DisplayThoughtBubbleMsg message:
                    DisplayThoughtBubble(message).Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(msg), msg, null);
            }
        }

        private void CancelCurrentAnimations()
        {
            _currentThoughtBubbleTween?.Kill();
            _currentProgressBarTween?.Kill();

            _thoughtBubbleTaskSource?.TrySetCanceled();
            _progressBarTaskSource?.TrySetCanceled();
        }

        private async UniTask DisplayThoughtBubble(DisplayThoughtBubbleMsg message)
        {
            ElementVisibility(_progressBarCont, false);
            ElementVisibility(_thoughtBubbleCont, true);

            _thoughtLab.text = message.ThoughtDataVo.LocalizedThought;

            _thoughtBubbleTaskSource = new UniTaskCompletionSource();

            _currentThoughtBubbleTween = DOTween.Sequence()
                .AppendInterval(message.DurationMs / 1000f)
                .Append(DOTween.To(
                    () => _thoughtBubbleCont.style.opacity.value,
                    x => _thoughtBubbleCont.style.opacity = x,
                    ProgressBarDefaultWidth,
                    ProgressBarShowHideDuration
                ).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    ElementVisibility(_thoughtBubbleCont, false);
                    _thoughtBubbleTaskSource.TrySetResult();
                });

            await _thoughtBubbleTaskSource.Task;
            _currentThoughtBubbleTween = null;
            _thoughtBubbleTaskSource = null;
        }

        private async UniTask DisplayProgressBar(DisplayProgressBarMsg message)
        {
            ElementVisibility(_thoughtBubbleCont, false);
            ElementVisibility(_progressBarCont, true);

            _actionLab.text = message.ActionName.ToUpper();
            var startWidth = 0f;
            var duration = message.Duration;

            _progressBarTaskSource = new UniTaskCompletionSource();

            _currentProgressBarTween = DOTween.Sequence()
                .Append(DOTween.To(
                    () => startWidth,
                    x =>
                    {
                        startWidth = x;
                        _progressCont.style.width = new Length(x);
                    },
                    _progressBarContWidth,
                    duration
                ).SetEase(Ease.Linear))
                .AppendCallback(() => message.CompletionSource.TrySetResult(EDialogResult.Close))
                .Append(DOTween.To(
                    () => _progressBarCont.style.opacity.value,
                    x => _progressBarCont.style.opacity = x,
                    ProgressBarDefaultWidth,
                    ProgressBarShowHideDuration
                ).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    ElementVisibility(_progressBarCont, false);
                    _progressBarTaskSource.TrySetResult();
                });

            await _progressBarTaskSource.Task;

            _currentProgressBarTween = null;
            _progressBarTaskSource = null;
        }

        private static void ElementVisibility(VisualElement element, bool value)
        {
            if (element == null)
                throw new NullReferenceException("Element is null. " + nameof(ElementVisibility));

            if (value)
            {
                element.style.opacity = 1f;
                element.style.display = DisplayStyle.Flex;
                return;
            }

            element.style.display = DisplayStyle.None;
        }

        private void OnGeometryLoaded(GeometryChangedEvent evt)
        {
            _progressBarContWidth = _progressBarCont.resolvedStyle.width - 2;
            _progressBarCont.UnregisterCallback<GeometryChangedEvent>(OnGeometryLoaded);
            _progressCont.style.width = ProgressBarDefaultWidth;

            ElementVisibility(_progressBarCont, false);
            ElementVisibility(_thoughtBubbleCont, false);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            CancelCurrentAnimations();
        }
    }
}
