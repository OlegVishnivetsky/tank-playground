using System.Collections;
using R3;
using Sirenix.OdinInspector;
using TankPlayground.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TankPlayground.Gameplay
{
    public abstract class ProjectileLauncherBase : NetworkBehaviour
    {
        [BoxGroup("Base")]
        [SerializeField] protected Animator ShootingFlashAnimator;
        [BoxGroup("Base")]
        [SerializeField] protected Collider2D OwnerCollider;
        [BoxGroup("Base")]
        [SerializeField] protected Transform ProjectileSpawnPoint;

        private IInputService _inputService;
        protected NetworkStatsController StatsController;

        public readonly NetworkVariable<bool> IsReloading = new();

        private bool _shouldLaunch;

        private float _fireRate;
        private float _reloadTime;

        private float _timeSinceLastFire = float.MaxValue;
        private float _serverLastFireTime;

        private const float ServerFireRateTimerTolerance = 0.05f;
        private const string ShootingFlashAnimationName = "ShootingFlash_Perform";

        [Inject]
        public void Construct(IInputService inputService) => _inputService = inputService;

        private void Awake() => StatsController = GetComponent<NetworkStatsController>();

        public override void OnNetworkSpawn()
        {
            _fireRate = StatsController.GetStatValue(StatType.FireRate);
            _reloadTime = StatsController.GetStatValue(StatType.ReloadTime);

            if (!IsOwner)
                return;

            _inputService.PrimaryFirePressed += OnPrimaryFirePressed;
            _inputService.ActionPressedObservable
                .Where(type => type == ActionButtonType.Reload)
                .Subscribe(_ => RequestReloadServerRpc())
                .AddTo(this);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;

            _inputService.PrimaryFirePressed -= OnPrimaryFirePressed;
        }

        private void Update()
        {
            OnUpdate();
            _timeSinceLastFire += Time.deltaTime;

            if (!IsOwner)
                return;

            if (StatsController.GetStatValue(StatType.Ammo) <= 0f && !IsReloading.Value)
            {
                RequestReloadServerRpc();
                return;
            }
            
            if (!CanShoot())
                return;
            
            if (_timeSinceLastFire < 1f / _fireRate)
                return;

            _timeSinceLastFire = 0f;

            PlayFlash();
            RequestFireServerRpc();
        }

        protected virtual bool CanShoot()
        {
            if (IsReloading.Value)
                return false;

            if (StatsController.GetStatValue(StatType.Ammo) <= 0)
                return false;

            return _shouldLaunch;
        }
        
        protected abstract void ServerFire();
        
        protected virtual void OnUpdate() { }

        protected void PlayFlash() => ShootingFlashAnimator.Play(ShootingFlashAnimationName, 0, 0f);
        private void OnPrimaryFirePressed(bool isPressed) => _shouldLaunch = isPressed;

        [ServerRpc]
        private void RequestFireServerRpc()
        {
            if (!StatsController.TryGetServerResource(StatType.Ammo, out ResourceStat ammoStat))
                return;
            
            if (ammoStat.Current.CurrentValue <= 0)
                return;

            if (Time.time - _serverLastFireTime < 1f / _fireRate - ServerFireRateTimerTolerance)
                return;

            _serverLastFireTime = Time.time;
            ammoStat.Decrease();
            ServerFire();
        }

        [ServerRpc]
        private void RequestReloadServerRpc()
        {
            if (IsReloading.Value)
                return;

            if (!StatsController.TryGetServerResource(StatType.Ammo, out ResourceStat ammoStat))
                return;

            if (ammoStat.Current.CurrentValue >= ammoStat.Max)
                return;

            IsReloading.Value = true;
            StartCoroutine(ReloadCoroutine());
        }

        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(_reloadTime);

            if (StatsController.TryGetServerResource(StatType.Ammo, out ResourceStat ammoStat))
                ammoStat.Restore();

            IsReloading.Value = false;
        }
    }
}