using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable.Impl
{
    /// <summary>
    /// Объект, у которого состояние переключается динамически в зависимости от глобальных сценарных состояний
    /// Взаимодействие только на уровне "подсказки", когда объект активен. (лужа мешает проходу - подсказка при взаимодействии)
    /// Меняет свое состояние. НЕ меняет глобальное сценарное состояние
    /// Пример: лужа на полу при отключении водоснабжения - "утекает" 
    /// </summary>
    public sealed class DynamicOnConditionSwitchable : ASwitchable<DynamicOnConditionSwitchSystem>
    {
    }

    public class DynamicOnConditionSwitchSystem : AInteractSystem<ISwitchable>
    {
        public DynamicOnConditionSwitchSystem(InteractSystemDepFlyweight dep) : base(dep)
        {
        }

        protected override UniTask<bool> OnInteractAsync()
        {
            Dep.Log.Warn("DynamicOnConditionSwitchSystem.OnInteractAsync");
            return UniTask.FromResult(true);
        }
    }
}
