using TankPlayground.Config;
using TankPlayground.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TankPlayground.Gameplay
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [SerializeField] private Animator _shootingFlashAnimator;
        [SerializeField] private Collider2D _ownerCollider;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private TankConfig _tankConfig;
        
        private bool _shouldLaunch;
        private float _timeSinceLastFire = float.MaxValue;
        private float _serverLastFireTime;
        
        private IInputService _inputService;

        private const float ServerFireRateTimerTolerance = 0.05f;
        private const string ShootingFlashAnimationName = "ShootingFlash_Perform";
        
        [Inject]
        public void Construct(IInputService inputService) => _inputService = inputService;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;
            
            _inputService.PrimaryFirePressed += OnPrimaryFirePressed;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;

            _inputService.PrimaryFirePressed -= OnPrimaryFirePressed;
        }

        private void Update()
        {
            _timeSinceLastFire += Time.deltaTime;
            
            if (!IsOwner || !_shouldLaunch)
                return;

            if (_timeSinceLastFire < 1f / _tankConfig.FireRate) 
                return;

            _timeSinceLastFire = 0f;
            PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
            LaunchProjectile(_tankConfig.ClientProjectilePrefab, 
                _projectileSpawnPoint.position, _projectileSpawnPoint.up, 0);
        }

        private void OnPrimaryFirePressed(bool isPressed)
        {
            if (!IsOwner)
                return;
            
            _shouldLaunch = isPressed;
        }

        [ServerRpc]
        private void PrimaryFireServerRpc(Vector2 position, Vector2 direction)
        {
            if (Time.time - _serverLastFireTime < 1f / _tankConfig.FireRate - ServerFireRateTimerTolerance) 
                return;

            _timeSinceLastFire = 0f;
            
            float distance = Vector2.Distance(position, _projectileSpawnPoint.position);
            
            if (distance > 2f) 
                return;
            
            direction = direction.normalized;
            
            LaunchProjectile(_tankConfig.ServerProjectilePrefab, position, direction, _tankConfig.Damage, false);
            SpawnDummyProjectileClientRpc(position, direction);
        }

        [ClientRpc]
        private void SpawnDummyProjectileClientRpc(Vector2 position, Vector2 direction)
        {
            if (IsOwner)
                return;
            
            LaunchProjectile(_tankConfig.ClientProjectilePrefab, position, direction, 0);
        }
        
        private void LaunchProjectile(
            ProjectileBase projectilePrefab,
            Vector2 position,
            Vector2 direction,
            int damage,
            bool showFlash = true)
        {
            ProjectileBase projectile = Instantiate(projectilePrefab,
                position, Quaternion.identity);
            projectile.Initialize(damage, _tankConfig.ProjectileSpeed, OwnerClientId);
            projectile.Shoot(_ownerCollider, direction);

            if (!showFlash)
                return;
            
            _shootingFlashAnimator.Play(ShootingFlashAnimationName, 0, 0f);
        }
    }
}