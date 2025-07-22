using _StoryGame.Game.Interact.todecor.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor
{
    public sealed class NewInteractable : ANewInteractable
    {
        protected async override UniTask ProcessPassiveDecorators()
        {
            _log.Warn("<color=yellow>Start Passive Decorators</color>");

            var prevState = CurrentState;
            foreach (var decorator in _passiveDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                _log.Warn($"{decorator.GetType().Name} / {decorator.Priority}");
                await decorator.ProcessPassive(this);
            }

            if (prevState != CurrentState)
            {
                _log.Warn(
                    $"<color=cyan>Кто-то изменил состояние с {prevState} на {CurrentState}. Перезапустить процесс пассивных декораторов</color>");
                ProcessPassiveDecorators();
            }

            _log.Warn("<color=yellow>End Passive Decorators</color>");
        }

        protected override async UniTask ProcessActiveDecorators()
        {
            _log.Warn("<color=green>Start Active Decorators</color>");
            var prevState = CurrentState;

            _log.Warn("ProcessActiveDecorators previous state: " + prevState + " / current state: " + CurrentState);
            foreach (var decorator in _activeDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                _log.Warn($"{decorator.GetType().Name} / {decorator.Priority}");
                await decorator.ProcessActive(this);
                // if (!result)
                // {
                //     // Показать сообщение (например, "Нужен лом"), остановить
                //     return;
                // }
            }

            _log.Warn("ProcessActiveDecorators previous state: " + prevState + " / current state: " + CurrentState);
            if (prevState != CurrentState)
            {
                _log.Warn(
                    $"<color=cyan>Кто-то изменил состояние с {prevState} на {CurrentState}. Перезапустить процесс пассивных декораторов</color>");
                ProcessPassiveDecorators();
            }

            _log.Warn("<color=green>End Active Decorators</color>");
        }
    }
}
