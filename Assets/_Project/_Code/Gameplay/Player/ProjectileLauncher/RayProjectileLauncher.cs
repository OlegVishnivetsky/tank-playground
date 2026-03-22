using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class RayProjectileLauncher : ProjectileLauncherBase
    {
        [BoxGroup("Ray")]
        [SerializeField] private float _maxDistance = 50f;
        [BoxGroup("Ray")]
        [SerializeField] private LaserProjectile _laserProjectilePrefab;

        protected override void ServerFire()
        {
            RaycastHit2D hit = Physics2D.Raycast(ProjectileSpawnPoint.position, 
                ProjectileSpawnPoint.up, _maxDistance);

            IDamageable target = null;
                
            if (hit)
                target = hit.collider.GetComponentInParent<IDamageable>();
                
            int damage = Mathf.RoundToInt(StatsController.GetStatValue(StatType.Damage));
                
            target?.TakeDamage(damage);
            LaunchProjectileClientRpc(hit ? hit.point : GetMaxHitPosition());
        }
        
        [ClientRpc]
        private void LaunchProjectileClientRpc(Vector2 hitPosition) => LaunchProjectile(hitPosition, false);

        private void LaunchProjectile(
            Vector2 hitPosition,
            bool isFlashShown = true)
        {
            LaserProjectile laser = Instantiate(_laserProjectilePrefab);
            laser.SetPositions(ProjectileSpawnPoint.position, hitPosition);
            laser.Shoot(OwnerCollider, ProjectileSpawnPoint.up);
            
            if (!isFlashShown)
                return;
            
            PlayFlash();
        }

        private Vector2 GetMaxHitPosition() => ProjectileSpawnPoint.position + ProjectileSpawnPoint.up * _maxDistance;
    }
}