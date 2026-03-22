using System.Collections.Generic;
using TankPlayground.Configs;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TankPlayground.Gameplay.UI
{
    public class StatusEffectsView : NetworkBehaviour
    {
        [SerializeField] private NetworkStatsController _statsController;
        [SerializeField] private StatusEffectItemView _itemViewPrefab;
        [SerializeField] private Transform _parentTransform;
        
        private StatusEffectsDatabase _effectsDatabase;
        
        private readonly List<StatusEffectItemView> _statusEffectItemViews = new();
        
        [Inject]
        public void Construct(StatusEffectsDatabase effectsDatabase) => _effectsDatabase = effectsDatabase;
        
        public override void OnNetworkSpawn() => _statsController.ActiveEffects.OnListChanged += OnActiveEffectsChanged;

        public override void OnNetworkDespawn() => _statsController.ActiveEffects.OnListChanged -= OnActiveEffectsChanged;

        private void OnActiveEffectsChanged(NetworkListEvent<NetworkStatusEffect> listEvent)
        {
            foreach (StatusEffectItemView itemView in _statusEffectItemViews)
            {
                if (itemView == null)
                    continue;
                
                Destroy(itemView.gameObject);
            }

            foreach (NetworkStatusEffect effect in _statsController.ActiveEffects)
            {
                StatusEffectConfig effectConfig = _effectsDatabase.GetStatusEffectConfig(effect.Name.ToString());
                StatusEffectItemView itemInstance = Instantiate(_itemViewPrefab, _parentTransform);
                itemInstance.Initialize(effect, effectConfig.Icon);
                _statusEffectItemViews.Add(itemInstance);
            }
        }
    }
}