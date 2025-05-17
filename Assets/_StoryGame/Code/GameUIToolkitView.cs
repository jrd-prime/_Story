using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame
{
    [RequireComponent(typeof(UIDocument))]
    public class GameUIToolkitView : MonoBehaviour
    {
        private VisualElement _ring;
        private Button _menuButton;
        private VisualElement RootVisualElement;
        private CompositeDisposable Disposables = new();

        [Inject] private FullScreenMovementViewModel ViewModel;

        private async void Awake()
        {
            var uiDoc = GetComponent<UIDocument>();
            await UIToolkitReadyAwaiter.WaitForReadyAsync(uiDoc);

            RootVisualElement = uiDoc.rootVisualElement;
            await UniTask.NextFrame();
            InitElements();
            await UniTask.NextFrame();
            Init();
        }

        protected void InitElements()
        {
            _ring = RootVisualElement.Q<VisualElement>(UIConst.FullScreenRingIDName);
            _menuButton = RootVisualElement.Q<Button>("menu-btn");
        }

        protected void Init()
        {
            if (ViewModel == null)
                throw new NullReferenceException("ViewModel is null in " + name);

            ViewModel.IsTouchPositionVisible.Subscribe(IsTouchPositionVisible).AddTo(Disposables);
            ViewModel.RingPosition.Subscribe(SetRingPosition).AddTo(Disposables);
            // ViewModel.ZoomScale.Subscribe(SetZoomScale).AddTo(Disposables);

            if (_menuButton != null)
            {
                _menuButton.clicked += OnMenuButtonClicked;
            }
        }

        private void SetRingPosition(Vector2 position)
        {
            _ring.style.left = position.x;
            _ring.style.top = position.y;
        }

        private void IsTouchPositionVisible(bool value)
        {
            _ring.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SetZoomScale(float scale)
        {
            Camera.main.orthographicSize = 5f / scale;
        }

        private void OnMenuButtonClicked()
        {
            Debug.Log("Menu button clicked!");
        }

        private void OnDestroy()
        {
            Disposables.Dispose();
        }
    }
}
