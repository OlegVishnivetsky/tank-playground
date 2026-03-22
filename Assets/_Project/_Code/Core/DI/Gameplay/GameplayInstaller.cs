using TankPlayground.Services;
using UnityEngine;
using Zenject;

namespace TankPlayground.Core.DI
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _tankPrefab;

        public override void InstallBindings()
        {
        }

        public override void Start()
        {
            base.Start();
            
            Container.Resolve<ISceneLoaderService>().NotifyReady();
        }
    }
}