using System;
using R3;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace TankPlayground.Gameplay.UI
{
    public class HealthView : NetworkBehaviour
    {
        [SerializeField] private Image _healthStartFillImage;
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private Image _healthEndFillImage;
        
        private TankEntity _entity;

        private void Awake() => _entity = GetComponentInParent<TankEntity>();

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
                return;
            
            _entity.StatsController.StatsChangedObservable
                .Where(data => data.type == StatType.Health)
                .Subscribe(data => UpdateView(data.stat))
                .AddTo(this);
            
            UpdateView(_entity.StatsController.GetNetworkStatValue(StatType.Health));
        }

        private void UpdateView(NetworkStatValue stat)
        {
            float maxHealth = stat.Max;
            float percent = stat.Current / maxHealth;

            _healthStartFillImage.fillAmount = Mathf.Clamp01(percent / 0.2f);
            _healthFillImage.fillAmount = Mathf.Clamp01((percent - 0.2f) / 0.6f);
            _healthEndFillImage.fillAmount = Mathf.Clamp01((percent - 0.8f) / 0.2f);
        }
    }
}