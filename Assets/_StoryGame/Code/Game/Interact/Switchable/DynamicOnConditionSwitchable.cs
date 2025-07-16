using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Switchable.Abstract;
using _StoryGame.Game.Interact.Switchable.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.Switchable
{
    /// <summary>
    /// Объект, у которого состояние переключается динамически в зависимости от глобальных сценарных состояний
    /// Взаимодействие только на уровне "подсказки", когда объект активен. (лужа мешает проходу - подсказка при взаимодействии)
    /// Меняет свое состояние. НЕ меняет глобальное сценарное состояние
    /// Пример: лужа на полу при отключении водоснабжения - "утекает" 
    /// </summary>
    public sealed class DynamicOnConditionSwitchable : ASwitchable<DynamicOnConditionSwitchSystem, IDynamicSwitchable>,
        IDynamicSwitchable
    {
        [Title(nameof(DynamicOnConditionSwitchable), titleAlignment: TitleAlignments.Centered)] [SerializeField]
        private bool inverseConditionImpact;

        [SerializeField] private string notFulfilledThoughtKey;

        public bool InverseConditionImpact => inverseConditionImpact;
        public string NotFulfilledThoughtKey => notFulfilledThoughtKey;
    }
}
