using System.Collections.Generic;
using TankPlayground.Config;
using TankPlayground.Gameplay;
using UnityEngine;

namespace TankPlayground.Configs
{
    [CreateAssetMenu(fileName = "Tank Config", menuName = "Configs/Gameplay/Tank Config")]
    public class TankConfig : ScriptableObject
    {
        public List<StatData> ResourceStats = new()
        {
            new(StatType.Health, 0f),
            new(StatType.Ammo, 0f),
            new(StatType.Shield, 0f, false)
        };
        
        public List<StatData> AttributesStats = new()
        {
            new(StatType.Damage, 0f),
            new(StatType.MoveSpeed, 0f),
            new(StatType.TurnRate, 0f),
            new(StatType.DamageTick, 0f),
            new(StatType.ShieldDamage, 0f),
            new(StatType.FireRate, 0f),
            new(StatType.ProjectileSpeed, 0f)
        };
    }
}