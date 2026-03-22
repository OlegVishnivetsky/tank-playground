using System;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    [Serializable]
    public class StatModifierBehaviour : IStatusEffectBehaviour
    {
        [SerializeField] private StatType _statType;
        [SerializeField] private ModifierType _modifierType;
        [SerializeField] private float _value;

        public void OnApply(StatusEffect effect, IStatCollection target)
        {
            if (target.TryGet(_statType, out AttributeStat stat))
                stat.AddModifier(new(_modifierType, _value, effect));
        }

        public void OnRemove(StatusEffect effect, IStatCollection target)
        {
            if (target.TryGet(_statType, out AttributeStat stat))
                stat.RemoveModifiersFromSource(effect);
        }

        public void Tick(StatusEffect effect, IStatCollection target, float deltaTime) { }
    }
}