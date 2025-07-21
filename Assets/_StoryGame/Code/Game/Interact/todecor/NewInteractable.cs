using _StoryGame.Game.Interact.todecor.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor
{
    public sealed class NewInteractable : ANewInteractable
    {
        protected override void ProcessPassiveDecorators()
        {
            Debug.LogWarning("--- ProcessPassiveDecorators called for " + gameObject.name);
            foreach (var decorator in _passiveDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                _log.Warn(
                    $"- UpdatePassiveState: decorator={decorator.GetType().Name} / priority={decorator.Priority}");
                decorator.ProcessPassive(this).Forget();
            }
        }

        protected override async UniTask ProcessActiveDecorators()
        {
            foreach (var decorator in _activeDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                _log.Warn($"- ActiveDecorator: decorator={decorator.GetType().Name} / priority={decorator.Priority}");
                var result = await decorator.ProcessActive(this);
                if (!result)
                {
                    // Показать сообщение (например, "Нужен лом"), остановить
                    return;
                }
            }
        }
    }
}
