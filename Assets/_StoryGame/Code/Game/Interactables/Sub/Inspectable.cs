using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _StoryGame.Game.Interactables
{
    /// <summary>
    /// Например, стеллаж (что не требует открывания, как сейф)
    /// </summary>
    public class Inspectable : Interactable
    {
        public override EInteractableType InteractableType => EInteractableType.Inspect;

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
