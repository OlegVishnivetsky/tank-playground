using System;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    [Serializable]
    public class DamageOverTimeBehaviour : IStatusEffectBehaviour
    {
        [SerializeField] private StatType _resourceType = StatType.Health;
        [SerializeField] private float _damagePerSecond = 5f;

        public void OnApply(StatusEffect effect, IStatCollection target) { }
        public void OnRemove(StatusEffect effect, IStatCollection target) { }

        public void Tick(StatusEffect effect, IStatCollection target, float deltaTime)
        {
            float damage = _damagePerSecond * effect.Stacks * deltaTime;
        
            if (target.TryGetResource(_resourceType, out ResourceStat resource))
                resource.Modify(-damage);
        }
    }
}