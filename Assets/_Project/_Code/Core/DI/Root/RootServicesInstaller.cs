using TankPlayground.Core.Network;
using TankPlayground.Core.StateMachine;
using TankPlayground.Services;
using TankPlayground.UI;
using UnityEngine;
using Zenject;

namespace TankPlayground.Core.DI
{
    public class RootServicesInstaller : MonoInstaller
    {
        [SerializeField] private LoadingScreen _loadingScreenPrefab;
        
        public override void InstallBindings()
        {
            BindLoadingScreen();
            BindStateFactory();
            BindAuthService();
            BindNetworkEntities();
            BindAssetProviderService();
            BindUINavigationService();
            BindSceneLoaderService();
            BindInputService();
            BindStateMachine();
        }

        public override void Start()
        {
            base.Start();
            Boot();
        }

        private void Boot()
        {
            IGameStateMachine stateMachine = Container.Resolve<IGameStateMachine>();
            BootState bootState = Container.Resolve<BootState>();
            
            stateMachine.RegisterState(bootState);
            stateMachine.SwitchTo<BootState>();
        }

        private void BindLoadingScreen() =>
            Container
                .Bind<LoadingScreen>()
                .FromComponentInNewPrefab(_loadingScreenPrefab)
                .AsSingle();
        
        private void BindStateFactory() => 
            Container
                .Bind<IStateFactory>()
                .To<StateFactory>()
                .AsSingle();

        private void BindAuthService() => 
            Container
                .Bind<IAuthService>()
                .To<AuthService>()
                .AsSingle();

        private void BindNetworkEntities()
        {
            Container
                .Bind<INetworkClient>()
                .To<NetworkClient>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<NetworkHost>()
                .AsSingle();
        }
        
        private void BindAssetProviderService() => 
            Container
                .Bind<IAssetProviderService>()
                .To<AssetProviderService>()
                .AsSingle();
        
        private void BindUINavigationService() =>
            Container
                .Bind<IUINavigationService>()
                .To<UINavigationService>()
                .AsSingle();

        private void BindSceneLoaderService() =>
            Container
                .Bind<ISceneLoaderService>()
                .To<SceneLoaderService>()
                .AsSingle();

        private void BindInputService() =>
            Container
                .BindInterfacesTo<InputService>()
                .AsSingle();

        private void BindStateMachine()
        {
            Container
                .Bind<BootState>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<GameStateMachine>()
                .AsSingle();
        }
    }
}