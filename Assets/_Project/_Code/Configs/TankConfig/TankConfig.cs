using Sirenix.OdinInspector;
using TankPlayground.Gameplay;
using UnityEngine;

namespace TankPlayground.Config
{
    [CreateAssetMenu(fileName = "Tank Config", menuName = "Configs/Gameplay/Tank Config")]
    public class TankConfig : ScriptableObject
    {
        [BoxGroup("Weapon")]
        public ProjectileBase ServerProjectilePrefab;
        [BoxGroup("Weapon")]
        public ProjectileBase ClientProjectilePrefab;
        [BoxGroup("Weapon")]
        public int Damage;
        [BoxGroup("Weapon")]
        public float ProjectileSpeed;
        [BoxGroup("Weapon")]
        public float FireRate;
    }
}