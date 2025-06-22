using System.Collections.Generic;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.Interact.Systems;
using _StoryGame.Game.UI.Abstract;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.Floating
{
    // TODO переделать
    public sealed class FloatingLayerHandler : AUIViewerHandlerBase, IUIViewerLayerHandler
    {
        private const string LeftId = "left";
        private const string CenterId = "center";
        private const string RightId = "right";

        private readonly Dictionary<EFloatingWindowType, VisualTreeAsset> _floatingWindowsAssets = new();
        private readonly Dictionary<EFloatingWindowType, VisualElement> _windows = new();
        private VisualElement _left;
        private VisualElement _center;
        private VisualElement _right;

        public FloatingLayerHandler(IObjectResolver resolver, VisualElement layerBack) : base(resolver, layerBack)
        {
        }

        protected override void PreInitialize()
        {
            var floatingWindowsData = UISettings.FloatingWindowDataVo;

            foreach (var windowData in floatingWindowsData.FloatingWindowDataVo)
                _floatingWindowsAssets.Add(windowData.eFloatingWindowType, windowData.visualTreeAsset);

            Log.Debug("FloatingLayerHandler initialized with " + _floatingWindowsAssets.Count + " windows.");
        }

        protected override void InitElements()
        {
            _left = MainContainer.GetVElement<VisualElement>(LeftId, nameof(FloatingLayerHandler));
            _center = MainContainer.GetVElement<VisualElement>(CenterId, nameof(FloatingLayerHandler));
            _right = MainContainer.GetVElement<VisualElement>(RightId, nameof(FloatingLayerHandler));

            InitializeWindows();
        }

        private void InitializeWindows()
        {
            foreach (var window in _floatingWindowsAssets)
            {
                var instance = window.Value.Instantiate();
                _windows.Add(window.Key, instance);
            }
        }

        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
        }

        // TODO CHECK IF ENOUGH ENERGY
        public void ShowHasLootWindow(ShowHasLootWindowMsg msg)
        {
            if (!HasWindow(msg.WindowType))
                throw new KeyNotFoundException($"No window for type: {msg.WindowType}");

            _center.Clear();

            var window = _windows[msg.WindowType];

            var title = window.GetVElement<Label>("title-label", window.name);
            var desc = window.GetVElement<Label>("desc-label", window.name);


            var close = window.GetVElement<Button>("close", window.name);
            var search = window.GetVElement<Button>("search", window.name);

            title.text = msg.InspectableData.LocalizedName;
            desc.text = msg.Tip;


            var closeHandler =
                new ClickCompletionHandler<EDialogResult>(close, EDialogResult.Close,
                    msg.CompletionSource, _center);
            var searchHandler =
                new ClickCompletionHandler<EDialogResult>(search, EDialogResult.Search,
                    msg.CompletionSource, _center);

            closeHandler.Register();
            searchHandler.Register();

            _center.Add(window);
        }

        public void ShowNoLootWindow(ShowNoLootWindowMsg msg)
        {
            if (!HasWindow(msg.WindowType))
                throw new KeyNotFoundException($"No window for type: {msg.WindowType}");

            _center.Clear();

            var window = _windows[msg.WindowType];

            var title = window.GetVElement<Label>("title-label", window.name);
            var desc = window.GetVElement<Label>("desc-label", window.name);
            var close = window.GetVElement<Button>("close", window.name);
            title.text = msg.ObjName;
            desc.text = msg.Tip;

            var closeHandler =
                new ClickCompletionHandler<EDialogResult>(close, EDialogResult.Close,
                    msg.CompletionSource, _center);

            closeHandler.Register();

            _center.Add(window);
        }

        private bool HasWindow(EFloatingWindowType windowType) => _windows.ContainsKey(windowType);

        public void ShowLootWindow(ShowLootWindowMsg msg)
        {
            if (!HasWindow(msg.WindowType))
                throw new KeyNotFoundException($"No window for type: {msg.WindowType}");

            _center.Clear();

            var window = _windows[msg.WindowType];

            var title = window.GetVElement<Label>("title-label", window.name);
            var close = window.GetVElement<Button>("close", window.name);
            var takeAll = window.GetVElement<Button>("take-all", window.name);

            title.text = msg.InspectableData.LocalizedName;

            //TODO переделать эту жесть, вообще весь класс
            int i = 0;
            foreach (var loot in msg.InspectableData.InspectablesLoot)
            {
                var i1 = window.GetVElement<VisualElement>("loot-item-container" + i, window.name);
                var ic1 = i1.GetVElement<VisualElement>("icon", window.name);
                ic1.style.backgroundImage = new StyleBackground(loot.Icon);
                i++;
            }

            var takeAllHandler =
                new ClickCompletionHandler<EDialogResult>(takeAll, EDialogResult.TakeAll,
                    msg.CompletionSource, _center);
            var closeHandler =
                new ClickCompletionHandler<EDialogResult>(close, EDialogResult.Close,
                    msg.CompletionSource, _center);

            takeAllHandler.Register();
            closeHandler.Register();

            _center.Add(window);
        }

        public void ShowNewNote(ShowNewNoteMsg msg)
        {
            Debug.Log("ShowNewNote");

            var note = msg.Loot.Currency as ANoteData;

            Debug.Log("ShowNewNote: " + note?.GetTextLocalizationKey());
            _left.Clear();

            var window = _windows[EFloatingWindowType.Note];

            var title = window.GetVElement<Label>("title-label", window.name);
            var desc = window.GetVElement<Label>("desc-label", window.name);
            var close = window.GetVElement<Button>("close", window.name);

            title.text = msg.Title;
            desc.text = msg.Text;

            _left.Add(window);
        }

        public void ShowRoomChooseWindow(ShowExitRoomWindowMsg msg)
        {
            _center.Clear();

            var window = _windows[EFloatingWindowType.LeaveRoom];

            var titleLab = window.GetVElement<Label>("title-label", window.name);
            var descLab = window.GetVElement<Label>("desc-label", window.name);
            var closeBtn = window.GetVElement<Button>("close", window.name);
            var leaveRoomBtn = window.GetVElement<Button>("leave-room", window.name);

            titleLab.text = msg.LocalizedName;
            descLab.text = msg.Question;

            var leaveRoomBtnHandler =
                new ClickCompletionHandler<EDialogResult>(leaveRoomBtn, EDialogResult.Apply,
                    msg.CompletionSource, _center);
            var closeBtnHandler =
                new ClickCompletionHandler<EDialogResult>(closeBtn, EDialogResult.Close,
                    msg.CompletionSource, _center);

            leaveRoomBtnHandler.Register();
            closeBtnHandler.Register();

            _center.Add(window);
        }

        public void DisplayArtefactInfoWindow(DisplayArtefactInfoMsg msg)
        {
            _center.Clear();

            var window = _windows[EFloatingWindowType.ArtefactInfo];

            var titleLab = window.GetVElement<Label>("title-label", window.name);
            var descLab = window.GetVElement<Label>("desc-label", window.name);
            var closeBtn = window.GetVElement<Button>("close", window.name);

            titleLab.text = msg.ConditionalLoot.Currency.LocalizationKey;
            descLab.text = Loca.Localize(msg.ConditionalLoot.Currency.DescriptionKey, ETable.SmallPhrase);

            var closeBtnHandler =
                new ClickCompletionHandler<EDialogResult>(closeBtn, EDialogResult.Close,
                    msg.CompletionSource, _center);

            closeBtnHandler.Register();

            _center.style.alignItems = Align.Center;
            _center.Add(window);
        }
    }

    public enum PositionType
    {
        Left,
        Center,
        Right
    }

    public enum EFloatingWindowType
    {
        HasLoot,
        NoLoot,
        Loot,
        Note,
        LeaveRoom,
        ArtefactInfo
    }
}
