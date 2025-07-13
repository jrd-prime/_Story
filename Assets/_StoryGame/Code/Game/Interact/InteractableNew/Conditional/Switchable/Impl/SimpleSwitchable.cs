using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable.Impl
{
    /// <summary>
    /// Объект, у которого можно переключить состояние. Выключатель, вентиль
    /// Может требовать выполнения каких-либо условий для взаимодействия.
    /// Меняет свое состояние и глобальное сценарное состояние
    /// </summary>
    public sealed class SimpleSwitchable : ASwitchable<SimpleSwitchSystem>
    {
    }

    public class SimpleSwitchSystem : AInteractSystem<ISwitchable>
    {
        public SimpleSwitchSystem(InteractSystemDepFlyweight dep) : base(dep)
        {
        }

        protected override UniTask<bool> OnInteractAsync()
        {
            Dep.Log.Warn("SimpleSwitchSystem.OnInteractAsync");
            return UniTask.FromResult(true);
        }
    }
}
