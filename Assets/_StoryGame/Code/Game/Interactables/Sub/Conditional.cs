using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Sub
{
    /// <summary>
    /// Объект в темном месте, который становится активным если есть фонарик
    /// </summary>
    public sealed class Conditional : Interactable
    {
        public override EInteractableType InteractableType => EInteractableType.Condition;

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
