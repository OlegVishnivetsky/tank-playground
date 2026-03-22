using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class BulletProjectile : ProjectileBase
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _collider;
        
        public override void Shoot(Collider2D ownerCollider, Vector2 direction)
        {
            transform.up = direction;
            Physics2D.IgnoreCollision(ownerCollider, _collider);
            _rigidbody.velocity = _rigidbody.transform.up * Speed;
        }
    }
}