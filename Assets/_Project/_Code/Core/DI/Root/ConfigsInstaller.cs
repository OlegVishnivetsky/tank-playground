using TankPlayground.Configs;
using UnityEngine;
using Zenject;

namespace TankPlayground.Core.DI
{
    public class ConfigsInstaller : MonoInstaller
    {
        [SerializeField] private UIScreenConfig _uiScreenConfig;
        [SerializeField] private StatusEffectsDatabase _effectsDatabase;
        
        public override void InstallBindings()
        {
            BindUIScreenConfig();
            BindStatusEffectsDatabase();
        }

        private void BindUIScreenConfig() =>
            Container
                .BindInstance(_uiScreenConfig)
                .AsSingle();
        
        private void BindStatusEffectsDatabase() =>
            Container
                .BindInstance(_effectsDatabase)
                .AsSingle();
    }
}