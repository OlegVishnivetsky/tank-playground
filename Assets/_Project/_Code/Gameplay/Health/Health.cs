using System;
using Unity.Netcode;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class Health : NetworkBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 100;
        
        public NetworkVariable<int> _health = new();
        private bool _isDead;
        
        public int CurrentHealth => _health.Value;
        public int MaxHealth => _maxHealth;
        
        public event Action<int, int> HealthChanged;
        public event Action<Health> Died;
        
        public override void OnNetworkSpawn()
        {
            _health.OnValueChanged += OnHealthChanged;

            if (IsServer)
                _health.Value = _maxHealth;
        }

        public override void OnNetworkDespawn() => _health.OnValueChanged -= OnHealthChanged;

        private void OnHealthChanged(int previousValue, int newValue) => HealthChanged?.Invoke(_health.Value, _maxHealth);

        public void TakeDamage(int amount) => Modify(-Mathf.Abs(amount));

        public void Restore(int amount) => Modify(Mathf.Abs(amount));

        private void Modify(int amount)
        {
            if (_isDead)
                return;
            
            int newHealth = _health.Value + amount;
            _health.Value = Mathf.Clamp(newHealth, 0, _maxHealth);
            

            if (_health.Value <= 0)
            {
                _isDead = true;
                Died?.Invoke(this);
            }
        }
    }
}