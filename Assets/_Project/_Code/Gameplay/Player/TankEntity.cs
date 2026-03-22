using Unity.Netcode;

namespace TankPlayground.Gameplay
{
    public class TankEntity : NetworkBehaviour
    {
        public Health Health { get; private set; }
        public TankAim Aim { get; private set; }
        public TankMovement Movement { get; private set; }
        public ProjectileLauncherBase Weapon {get; private set;}
        public NetworkStatsController StatsController { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            Aim = GetComponent<TankAim>();
            Movement = GetComponent<TankMovement>();
            Weapon = GetComponent<ProjectileLauncherBase>();
            StatsController = GetComponent<NetworkStatsController>();
        }
    }
}