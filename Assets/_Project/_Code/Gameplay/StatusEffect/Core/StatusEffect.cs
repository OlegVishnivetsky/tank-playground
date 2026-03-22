using TankPlayground.Configs;

namespace TankPlayground.Gameplay
{
    public class StatusEffect
    {
        private readonly IStatusEffectBehaviour _behaviour;
        
        public StatusEffectConfig Config { get; }
        public float RemainingDuration { get; private set; }
        public int Stacks { get; private set; } = 1;

        public StatusEffect(StatusEffectConfig config)
        {
            Config = config;
            RemainingDuration = config.Duration;
            _behaviour = config.Behaviour;
        }

        public void AddStack()
        {
            if (Stacks < Config.MaxStacks)
                Stacks++;
        }

        public void ResetDuration() => RemainingDuration = Config.Duration;

        public void ExtendDuration(float extra) => RemainingDuration += extra;
        
        public void Apply(IStatCollection target) => _behaviour.OnApply(this, target);

        public void Tick(IStatCollection target, float deltaTime)
        {
            if (Config.Duration > 0)
                RemainingDuration -= deltaTime;

            _behaviour.Tick(this, target, deltaTime);
        }

        public void Remove(IStatCollection target) => _behaviour.OnRemove(this, target);
    }
}