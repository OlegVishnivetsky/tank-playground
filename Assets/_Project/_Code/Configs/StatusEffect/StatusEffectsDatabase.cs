using System.Collections.Generic;
using UnityEngine;
using ZLinq;

namespace TankPlayground.Configs
{
    [CreateAssetMenu(fileName = "StatusEffects Database", menuName = "Configs/Gameplay/StatusEffects Database")]
    public class StatusEffectsDatabase : ScriptableObject
    {
        public List<StatusEffectConfig> StatusEffects;

        public StatusEffectConfig GetStatusEffectConfig(string effectName) =>
            StatusEffects
                .AsValueEnumerable()
                .FirstOrDefault(e => e.Name == effectName);
    }
}