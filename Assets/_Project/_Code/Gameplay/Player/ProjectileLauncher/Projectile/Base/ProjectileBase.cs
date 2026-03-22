using UnityEngine;
using R3;
using R3.Triggers;
using TankPlayground.Configs;

namespace TankPlayground.Gameplay
{
    public abstract class ProjectileBase : MonoBehaviour, IProjectile
    {
        [SerializeField] private StatusEffectConfig _effectOnHit;
        
        public ulong OwnerClientId { get; private set; }
        public int Damage { get; private set; }
        public float Speed { get; private set; }

        private void Awake() =>
            this.OnTriggerEnter2DAsObservable()
                .Select(other => other.GetComponentInParent<IDamageable>())
                .Where(damageable => damageable != null)
                .Where(netObj => netObj.OwnerClientId != OwnerClientId)
                .Subscribe(damageable =>
                {
                    if (Damage <= 0)
                        return;
                    
                    damageable.TakeDamage(Damage);
                    
                    if (_effectOnHit == null)
                        return;
                    
                    damageable.ApplyEffect(_effectOnHit);
                })
                .AddTo(this);

        public void Initialize(int damage, float speed, ulong ownerId)
        {
            Damage = damage;
            Speed = speed;
            OwnerClientId = ownerId;
        }
        
        public abstract void Shoot(Collider2D ownerCollider, Vector2 direction);
    }
}