using System;
using System.Collections.Generic;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.Interactables.Inspect;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using _StoryGame.Game.UI.Messages;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Viewer
{
    public sealed class FloatingWindowFactory
    {
        private readonly Dictionary<FloatingWindowType, VisualElement> _windows;
        private Button baseCloseButton;

        public FloatingWindowFactory(Dictionary<FloatingWindowType, VisualElement> windows)
        {
            _windows = windows;
        }

        public VisualElement CreateAndFill(ShowFloatingWindowMsg<DialogResult> msg)
        {
            VisualElement window = null;
            switch (msg.FloatingWindowType)
            {
                case FloatingWindowType.HasLoot:
                    if (msg.WindowData is not HasLootWindowData hasLootWindowData)
                        throw new Exception(
                            $"WindowData ({typeof(HasLootWindowData)}) does not match the FloatingWindowType ({nameof(FloatingWindowType)}).");
                    window = _windows[FloatingWindowType.HasLoot];
                    PrepareHasLootWindow(window, msg);
                    break;
                case FloatingWindowType.NoLoot:
                    break;
            }

            return window;
        }

        private void PrepareHasLootWindow(VisualElement window, ShowFloatingWindowMsg<DialogResult> data)
        {
            PrepareBase(data.WindowData.Title, window, data.CompletionSource);
        }

        private void PrepareBase(string title, VisualElement window,
            UniTaskCompletionSource<DialogResult> completionSource)
        {
            var titleLabel = window.GetVisualElement<Label>("title", window.name);
            titleLabel.text = title;

            baseCloseButton = window.GetVisualElement<Button>("baseCloseBtn", window.name);
            baseCloseButton.RegisterCallback<ClickEvent>(_ => OnBaseClose(completionSource));
        }

        private void OnBaseClose(UniTaskCompletionSource<DialogResult> source)
        {
            source.TrySetResult(DialogResult.Close);
            baseCloseButton.UnregisterCallback<ClickEvent>(_ => OnBaseClose(source));
        }
    }

    public record HasLootWindowData(string Title) : IFloatingWindowData
    {
        public string Title { get; } = Title;
    }
}
