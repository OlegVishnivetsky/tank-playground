using TankPlayground.Services;
using Zenject;

namespace TankPlayground.Core.DI
{
    public class RootServicesInstaller : MonoInstaller
    {
        public override void InstallBindings() => BindInputService();

        private void BindInputService() =>
            Container
                .BindInterfacesTo<InputService>()
                .AsSingle();
    }
}