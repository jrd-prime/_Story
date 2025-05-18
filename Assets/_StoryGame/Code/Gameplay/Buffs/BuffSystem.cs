using System.Collections.Generic;
using Game.Buffs.Interfaces;
using Game.Core.Interfaces;
using UnityEngine;
using VContainer.Unity;

namespace Game.Buffs
{
    public sealed class BuffSystem : IFixedTickable
    {
        private readonly Dictionary<IBuffable, Dictionary<BuffType, IBuff>> _activeBuffs = new();

        public void ApplyBuff(IBuffable target, IBuff buff)
        {
            if (!_activeBuffs.ContainsKey(target))
                _activeBuffs[target] = new Dictionary<BuffType, IBuff>();

            var debuffs = _activeBuffs[target];
            var debuffType = buff.BuffType;

            if (debuffs.ContainsKey(debuffType))
            {
                debuffs[debuffType].RemoveDebuff(target);
                debuffs.Remove(debuffType);
            }

            buff.ApplyDebuff(target);
            debuffs[debuffType] = buff;
        }

        public void RemoveDebuff(IBuffable target, BuffType buffType)
        {
            if (!_activeBuffs.TryGetValue(target, out var targetBuffs)) return;

            if (!targetBuffs.ContainsKey(buffType)) return;

            targetBuffs[buffType].RemoveDebuff(target);
            targetBuffs.Remove(buffType);

            if (targetBuffs.Count == 0)
                _activeBuffs.Remove(target);
        }

        // TODO подумать как уменьшить количество вызовов в самих реализациях дебафа
        public void FixedTick()
        {
            var targets = new List<IBuffable>(_activeBuffs.Keys);
            foreach (var target in targets)
            {
                var debuffs = _activeBuffs[target];
                var expiredDebuffs = new List<BuffType>();

                foreach (var effect in debuffs.Values)
                {
                    effect.UpdateDebuff(target, Time.deltaTime);

                    if (!effect.IsActive)
                        expiredDebuffs.Add(effect.BuffType);
                }

                foreach (var debuffType in expiredDebuffs)
                {
                    debuffs[debuffType].RemoveDebuff(target);
                    debuffs.Remove(debuffType);
                }

                if (debuffs.Count == 0)
                    _activeBuffs.Remove(target);
            }
        }
    }
}
