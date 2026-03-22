using TankPlayground.Configs;
using Unity.Netcode;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class Health : NetworkBehaviour, IDamageable
    {
        private TankEntity _entity;

        private void Awake() => _entity = GetComponent<TankEntity>();

        public void TakeDamage(int amount)
        {
            if (!IsServer)
            {
                Debug.LogError("TakeDamage should only be called on the server");
                return;
            }

            if (!_entity.StatsController.TryGetServerResource(StatType.Health, out ResourceStat health))
                return;

            if (amount <= 0 || amount > health.Max)
                return;

            health.Modify(-amount);
        }
        
        public void ApplyEffect(StatusEffectConfig effect) => _entity.StatsController.ApplyEffect(effect);
    }
}