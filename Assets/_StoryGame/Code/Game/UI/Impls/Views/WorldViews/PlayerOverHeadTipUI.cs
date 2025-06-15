using System;
using _StoryGame.Core.Interfaces.Publisher.Messages;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.Interactables.Data;
using DG.Tweening;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Views.WorldViews
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(UIDocument))]
    public sealed class PlayerOverHeadTipUI : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private int panelWidth = 1600;
        [SerializeField] private int panelHeight = 720;
        [SerializeField] private float panelScale = 1f;
        [SerializeField] private float pixelsPerUnit = 500f;
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;
        [SerializeField] private RenderTexture renderTextureAsset;

        private UIDocument _uiDocument;
        private Label _nameLabel;
        private Label _tipLabel;
        private VisualElement _root;
        private VisualElement _progress;
        private VisualElement pcont;

        private float contWidth;
        private readonly CompositeDisposable _disposables = new();
        private bool isVisible = false;
        private VisualElement _barCont;
        private Label _actionLab;

        [Inject]
        private void Construct(ISubscriber<IPlayerActionMsg> subscriber)
        {
            subscriber.Subscribe(
                OnMessage,
                msg => msg is ShowUIProgressOnPlayerActionMsg
            ).AddTo(_disposables);
        }

        private void Start()
        {
            _uiDocument = GetComponent<UIDocument>();
            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            var panelSettings = Instantiate(panelSettingsAsset);
            panelSettings.name = "PlayerOverheadTipPanelSettings";

            var initializer = new TipInitializer();
            initializer.Init(new WorldTipData(_uiDocument, visualTreeAsset, transform, meshFilter, meshRenderer,
                renderTextureAsset, panelSettings,
                panelWidth, panelHeight, panelScale, pixelsPerUnit
            ));

            _root = _uiDocument.rootVisualElement;


            _barCont = _root.GetVisualElement<VisualElement>("barC", name);
            _progress = _root.GetVisualElement<VisualElement>("progress", name);
            pcont = _root.GetVisualElement<VisualElement>("p-cont", name);
            _actionLab = _root.GetVisualElement<Label>("action-label", name);

            pcont.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void LateUpdate()
        {
            if (!mainCamera || !isVisible)
                return;

            transform.LookAt(mainCamera.transform.position);
            transform.Rotate(0, 180f, 0);
        }

        private void OnMessage(IPlayerActionMsg msg)
        {
            var message = msg as ShowUIProgressOnPlayerActionMsg ??
                          throw new ArgumentException("Unexpected msg type", nameof(msg));
            _actionLab.text = message.ActionName.ToUpper();
            ShowRoot();
            var startWidth = 0f;
            var duration = message.Duration;

            DOTween
                .To(
                    () => startWidth,
                    x =>
                    {
                        startWidth = x;
                        _progress.style.width = new Length(x);
                    },
                    contWidth,
                    duration
                )
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    message.CompletionSource.TrySetResult(EDialogResult.Close);

                    DOTween.To(
                            () => (float)_barCont.style.opacity.value,
                            x => _barCont.style.opacity = x,
                            0f,
                            0.5f
                        ).SetEase(Ease.Linear)
                        .OnComplete(HideRoot);
                });

            Debug.Log("PlayerOverHeadTipUI - " + message.ActionName + " complete");
        }

        private void ShowRoot()
        {
            _barCont.style.display = DisplayStyle.Flex;
            _barCont.style.opacity = 1f;
            isVisible = true;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            contWidth = pcont.resolvedStyle.width - 2;
            pcont.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            _progress.style.width = 0f;
            HideRoot();
        }

        private void HideRoot()
        {
            _barCont.style.display = DisplayStyle.None;
            isVisible = false;
        }

        private void OnDestroy() => _disposables.Dispose();
    }
}
