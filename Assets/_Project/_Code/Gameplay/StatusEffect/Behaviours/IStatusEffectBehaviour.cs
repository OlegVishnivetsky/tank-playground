namespace TankPlayground.Gameplay
{
    public interface IStatusEffectBehaviour
    {
        void OnApply(StatusEffect effect, IStatCollection target);
        void OnRemove(StatusEffect effect, IStatCollection target);
        void Tick(StatusEffect effect, IStatCollection target, float deltaTime);
    }
}