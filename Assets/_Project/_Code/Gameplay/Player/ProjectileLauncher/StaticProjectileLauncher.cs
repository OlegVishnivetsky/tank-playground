using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class StaticProjectileLauncher : ProjectileLauncherBase
    {
        [BoxGroup("Prefabs")]
        [SerializeField] private ProjectileBase _serverProjectilePrefab;
        [BoxGroup("Prefabs")]
        [SerializeField] private GameObject _clientProjectilePrefab;
        
        private GameObject _currentDummyProjectile;

        protected override void OnUpdate()
        {
            if (!CanShoot() && _currentDummyProjectile != null)
                Destroy(_currentDummyProjectile);
        }

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
            SpawnDummyProjectile(position, direction);

        private void LaunchProjectile(
            ProjectileBase projectilePrefab,
            Vector2 position,
            Vector2 direction,
            int damage,
            bool showFlash = true)
        {
            float projectileSpeed = StatsController.GetStatValue(StatType.ProjectileSpeed);
            ProjectileBase projectile = Instantiate(projectilePrefab, position, Quaternion.identity);
            projectile.transform.up = direction;
            projectile.Initialize(damage, projectileSpeed, OwnerClientId);
            projectile.Shoot(OwnerCollider, direction);

            if (!showFlash)
                return;

            PlayFlash();
        }

        private void SpawnDummyProjectile(Vector2 position, Vector2 direction)
        {
            if (_currentDummyProjectile != null)
                return;
            
            _currentDummyProjectile = Instantiate(_clientProjectilePrefab, position, Quaternion.identity);
            _currentDummyProjectile.transform.SetParent(ProjectileSpawnPoint);
            _currentDummyProjectile.transform.localPosition = Vector3.zero;
            _currentDummyProjectile.transform.up = direction;
        }
    }
}