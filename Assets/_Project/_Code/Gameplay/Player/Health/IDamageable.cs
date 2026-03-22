using TankPlayground.Configs;

namespace TankPlayground.Gameplay
{
    public interface IDamageable
    {
        ulong OwnerClientId { get; }

        void TakeDamage(int amount);
        void ApplyEffect(StatusEffectConfig effect);
    }
}