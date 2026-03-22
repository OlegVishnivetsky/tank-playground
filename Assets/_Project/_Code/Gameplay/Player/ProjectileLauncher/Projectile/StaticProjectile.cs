using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class StaticProjectile : ProjectileBase
    {
        [SerializeField] private Collider2D _collider;
        
        private const float Lifetime = 0.15f;
        
        public override void Shoot(Collider2D ownerCollider, Vector2 direction)
        {
            transform.up = direction;
            Physics2D.IgnoreCollision(ownerCollider, _collider);
            Destroy(gameObject, Lifetime);
        }
    }
}