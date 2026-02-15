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
        [SerializeField] private GameObject _clientProjectilePrefab;
        [SerializeField] private GameObject _serverProjectilePrefab;
        
        [Header("Settings")]
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private float _fireRate = 1f;
        
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

            if (_timeSinceLastFire < 1f / _fireRate) 
                return;

            _timeSinceLastFire = 0f;
            PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
            LaunchProjectile(_clientProjectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.up);
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
            if (Time.time - _serverLastFireTime < 1f / _fireRate - ServerFireRateTimerTolerance) 
                return;

            _timeSinceLastFire = 0f;
            
            float distance = Vector2.Distance(position, _projectileSpawnPoint.position);
            
            if (distance > 2f) 
                return;
            
            direction = direction.normalized;
            
            LaunchProjectile(_serverProjectilePrefab, position, direction, false);
            SpawnDummyProjectileClientRpc(position, direction);
        }

        [ClientRpc]
        private void SpawnDummyProjectileClientRpc(Vector2 position, Vector2 direction)
        {
            if (IsOwner)
                return;
            
            LaunchProjectile(_clientProjectilePrefab, position, direction);
        }
        
        private void LaunchProjectile(
            GameObject projectilePrefab,
            Vector2 position,
            Vector2 direction,
            bool showFlash = true)
        {
            GameObject projectile = Instantiate(projectilePrefab,
                position, Quaternion.identity);
            projectile.transform.up = direction;
            
            Physics2D.IgnoreCollision(_ownerCollider, projectile.GetComponent<Collider2D>());

            if (projectile.TryGetComponent(out Rigidbody2D rb))
            {
                Debug.Log($"Move projectile: {rb.transform.up * _projectileSpeed}");
                rb.velocity = rb.transform.up * _projectileSpeed;
            }

            if (!showFlash)
                return;
            
            _shootingFlashAnimator.Play(ShootingFlashAnimationName, 0, 0f);
        }
    }
}