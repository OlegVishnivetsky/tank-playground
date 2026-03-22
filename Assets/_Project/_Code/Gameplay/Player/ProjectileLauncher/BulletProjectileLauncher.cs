using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class BulletProjectileLauncher : ProjectileLauncherBase
    {
        [BoxGroup("Prefabs")]
        [SerializeField] private ProjectileBase _serverProjectilePrefab;

        [BoxGroup("Prefabs")]
        [SerializeField] private ProjectileBase _clientProjectilePrefab;

        protected override void ServerFire()
        {
            float distance = Vector2.Distance(ProjectileSpawnPoint.position, ProjectileSpawnPoint.position);

            if (distance > 2f)
                return;

            Vector2 normalizedDirection = ProjectileSpawnPoint.up;
            normalizedDirection = normalizedDirection.normalized;

            int damage = Mathf.RoundToInt(StatsController.GetStatValue(StatType.Damage));

            LaunchProjectile(_serverProjectilePrefab, ProjectileSpawnPoint.position, normalizedDirection, damage);
            SpawnDummyProjectileClientRpc(ProjectileSpawnPoint.position, normalizedDirection);
        }

        [ClientRpc]
        private void SpawnDummyProjectileClientRpc(Vector2 position, Vector2 direction) =>
            LaunchProjectile(_clientProjectilePrefab, position, direction, 0);

        private void LaunchProjectile(
            ProjectileBase projectilePrefab,
            Vector2 position,
            Vector2 direction,
            int damage,
            bool showFlash = true)
        {
            float projectileSpeed = StatsController.GetStatValue(StatType.ProjectileSpeed);
            ProjectileBase projectile = Instantiate(projectilePrefab,
                position, Quaternion.identity);
            projectile.Initialize(damage, projectileSpeed, OwnerClientId);
            projectile.Shoot(OwnerCollider, direction);

            if (!showFlash)
                return;

            PlayFlash();
        }
    }
}