using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Movement;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers
{
    public sealed class MovementLayerHandler : UIViewerHandlerBase, IUIViewerLayerHandler
    {
        private const string MovementAreaId = "movement-area";
        private const string RingId = "ring";

        private VisualElement _movementArea;

        // private VisualElement _ring;
        private IMovementHandler _movementHandler;

        public MovementLayerHandler(IObjectResolver resolver, VisualElement layerBack) : base(resolver, layerBack)
        {
        }

        protected override void ResolveDependencies(IObjectResolver resolver)
        {
            _movementHandler = resolver.Resolve<IMovementHandler>();
        }

        protected override void InitElements()
        {
            _movementArea = GetElement<VisualElement>(MovementAreaId);
            // _ring = GetElement<VisualElement>(RingId);
        }

        protected override void Subscribe()
        {
            _movementHandler.IsTouchVisible.Subscribe(IsTouchPositionVisible).AddTo(Disposables);
            _movementHandler.RingPosition.Subscribe(SetRingPosition).AddTo(Disposables);

            // _movementArea.RegisterCallback<PointerDownEvent>(_movementHandler.OnPointerDown);
            // _movementArea.RegisterCallback<PointerMoveEvent>(_movementHandler.OnPointerMove);
            // _movementArea.RegisterCallback<PointerUpEvent>(_movementHandler.OnPointerUp);
            // _movementArea.RegisterCallback<PointerOutEvent>(_movementHandler.OnPointerCancel);
        }

        protected override void Unsubscribe()
        {
            // _movementArea.UnregisterCallback<PointerDownEvent>(_movementHandler.OnPointerDown);
            // _movementArea.UnregisterCallback<PointerMoveEvent>(_movementHandler.OnPointerMove);
            // _movementArea.UnregisterCallback<PointerUpEvent>(_movementHandler.OnPointerUp);
            // _movementArea.UnregisterCallback<PointerOutEvent>(_movementHandler.OnPointerCancel);
        }

        private void SetRingPosition(Vector2 position)
        {
            // _ring.style.left = position.x;
            // _ring.style.top = position.y;
        }

        private void IsTouchPositionVisible(bool value)
        {
            // _ring.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
