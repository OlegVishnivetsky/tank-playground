using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace TankPlayground.Gameplay.UI
{
    public class HealthView : NetworkBehaviour
    {
        [SerializeField] private Health _health;
        
        [Space(10f)]
        [SerializeField] private Image _healthStartFillImage;
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private Image _healthEndFillImage;

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
                return;

            _health.HealthChanged += OnHealthChanged;
            OnHealthChanged(_health.CurrentHealth, _health.MaxHealth);
        }
        
        public override void OnNetworkDespawn()
        {
            if (!IsClient)
                return;
            
            _health.HealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int current, int maxHealth)
        {
            float percent = (float)current / maxHealth;
            
            Debug.Log($"Health: {percent}");

            _healthStartFillImage.fillAmount = Mathf.Clamp01(percent / 0.2f);
            _healthFillImage.fillAmount = Mathf.Clamp01((percent - 0.2f) / 0.6f);
            _healthEndFillImage.fillAmount = Mathf.Clamp01((percent - 0.8f) / 0.2f);
        }
    }
}