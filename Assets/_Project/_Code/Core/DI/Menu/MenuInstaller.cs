using TankPlayground.Core.StateMachine;
using Zenject;

namespace TankPlayground.Core.DI
{
    public class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindMenuStates();
        }

        public override void Start()
        {
            base.Start();
            
            IGameStateMachine stateMachine = Container.Resolve<IGameStateMachine>();
            MenuState menuState = Container.Resolve<MenuState>();
            
            stateMachine.RegisterState(menuState);
            stateMachine.SwitchTo<MenuState>();
        }

        private void BindMenuStates() =>
            Container
                .Bind<MenuState>()
                .AsSingle();
    }
}