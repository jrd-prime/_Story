using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.UI.Const;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data;
using _StoryGame.Data.UI;
using _StoryGame.Game.Extensions;
using _StoryGame.Infrastructure.AppStarter;
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

        [SerializeField] private Shader transparentShader;
        [SerializeField] private Shader textureShader;

        private const float ShowDuration = 0.3f;
        private const float HideDuration = 0.5f;

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
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private PanelSettings panelSettings;
        private IJLog _log;

        [Inject]
        private void Construct(IJLog log, ISubscriber<IPlayerOverHeadUIMsg> subscriber, AppStartHandler appStartHandler)
        {
            _log = log;
            subscriber.Subscribe(OnMessage).AddTo(_disposables);
            appStartHandler.IsAppStarted.Subscribe(OnAppStarted).AddTo(_disposables);
        }

        private void OnAppStarted(Unit _)
        {
            _uiDocument = GetComponent<UIDocument>();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            panelSettings = Instantiate(panelSettingsAsset);
            panelSettings.name = "PlayerOverheadTipPanelSettings";


            var initializer = new TipInitializer(_log);
            initializer.Init(new WorldTipData(_uiDocument, visualTreeAsset, transform, meshFilter, meshRenderer,
                renderTextureAsset, panelSettings, panelWidth, panelHeight, panelScale, pixelsPerUnit,
                transparentShader, textureShader));

            _root = _uiDocument.rootVisualElement;
            _mainCont = _root.GetVElement<VisualElement>(OverHeadUIConst.MainCont, name);

            InitProgressBarElements();
            InitThoughtBubbleElements();

            _progressBarCont.RegisterCallback<GeometryChangedEvent>(OnGeometryLoaded);
        }

        private void Start()
        {
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

        private async UniTask DisplayThoughtBubble(DisplayThoughtBubbleMsg message)
        {
            ElementVisibility(_progressBarCont, false);
            ElementVisibility(_thoughtBubbleCont, true);

            _thoughtLab.text = "  " + message.ThoughtDataVo.LocalizedThought;
            _thoughtBubbleTaskSource = new UniTaskCompletionSource();

            _currentThoughtBubbleTween = DOTween
                .Sequence()
                .Append(ChangeOpacityAnimation(_thoughtBubbleCont, 1f, ShowDuration))
                .AppendInterval(message.DurationMs / 1000f)
                .Append(ChangeOpacityAnimation(_thoughtBubbleCont, 0f, HideDuration))
                .SetEase(Ease.Linear)
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
            _progressCont.style.width = startWidth;
            var duration = message.Duration;

            _progressBarTaskSource = new UniTaskCompletionSource();

            _currentProgressBarTween = DOTween
                .Sequence()
                .Append(ChangeOpacityAnimation(_progressBarCont, 1f, ShowDuration))
                .Append(FillProgressBarAnimation(startWidth, duration))
                .SetEase(Ease.Linear)
                .AppendCallback(() => message.CompletionSource.TrySetResult(EDialogResult.Close))
                .Append(ChangeOpacityAnimation(_progressBarCont, 0f, HideDuration))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ElementVisibility(_progressBarCont, false);
                    _progressBarTaskSource.TrySetResult();
                });

            await _progressBarTaskSource.Task;

            _currentProgressBarTween = null;
            _progressBarTaskSource = null;
        }

        private static Tween ChangeOpacityAnimation(VisualElement element, float endValue, float duration) =>
            DOTween.To(
                () => element.style.opacity.value,
                x => element.style.opacity = x,
                endValue,
                duration);

        private Tween FillProgressBarAnimation(float startWidth, float duration) =>
            DOTween.To(
                () => startWidth,
                x =>
                {
                    startWidth = x;
                    _progressCont.style.width = new Length(x);
                },
                _progressBarContWidth,
                duration);


        private static void ElementVisibility(VisualElement element, bool value)
        {
            if (element == null)
                throw new NullReferenceException("Element is null. " + nameof(ElementVisibility));

            if (value)
            {
                element.style.opacity = 0f;
                element.style.display = DisplayStyle.Flex;
                return;
            }

            element.style.display = DisplayStyle.None;
        }

        private void OnGeometryLoaded(GeometryChangedEvent evt)
        {
            _progressBarContWidth = _progressBarCont.resolvedStyle.width - 2;
            _progressBarCont.UnregisterCallback<GeometryChangedEvent>(OnGeometryLoaded);
            _progressCont.style.width = 0f;

            ElementVisibility(_progressBarCont, false);
            ElementVisibility(_thoughtBubbleCont, false);
        }

        private void CancelCurrentAnimations()
        {
            _currentThoughtBubbleTween?.Kill();
            _currentProgressBarTween?.Kill();

            _thoughtBubbleTaskSource?.TrySetCanceled();
            _progressBarTaskSource?.TrySetCanceled();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            CancelCurrentAnimations();
        }
    }
}
