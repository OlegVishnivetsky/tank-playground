using UnityEngine;

namespace TankPlayground.Gameplay
{
    public interface IProjectile
    {
        void Shoot(Collider2D ownerCollider, Vector2 direction);
    }
}