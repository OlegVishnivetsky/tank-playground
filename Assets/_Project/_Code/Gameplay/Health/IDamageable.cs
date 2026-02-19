namespace TankPlayground.Gameplay
{
    public interface IDamageable
    {
        ulong OwnerClientId { get; }

        void TakeDamage(int amount);
    }
}