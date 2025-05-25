using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Sub
{
    /// <summary>
    /// Например, дверь
    /// </summary>
    public sealed class Usable : AInteractable
    {
        public override EInteractableType InteractableType => EInteractableType.Use;

        public override async UniTask InteractAsync(ICharacter character)
        {
            var completionSource = new UniTaskCompletionSource();

            transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .OnComplete(() => completionSource.TrySetResult());

            await completionSource.Task;
        }
    }
}
