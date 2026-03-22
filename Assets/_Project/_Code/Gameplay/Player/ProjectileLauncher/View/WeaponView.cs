using System;
using DG.Tweening;
using R3;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace TankPlayground.Gameplay.UI
{
    public class WeaponView : NetworkBehaviour
    {
        [SerializeField] private RectTransform _reloadingIndicator;
        [SerializeField] private RectTransform _ammoIndicatorPrefab;
        [SerializeField] private RectTransform _ammoIndicatorParent;
        [SerializeField] private Image _ammoFillImage;
        [SerializeField] private GameObject _indicatorModeObject;
        [SerializeField] private GameObject _fillAmountModeObject;

        private TankEntity _entity;
        private Tween _reloadingTween;
        
        private float _ammoIndicatorWidth;
        
        private const float FillAmountModeThreshold = 7f;
        private const float ReloadIconAnimationDuration = 1f;

        private void Awake() => _entity = GetComponentInParent<TankEntity>();

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
                return;
            
            _entity.Weapon.IsReloading.OnValueChanged += OnReload;
            _entity.StatsController.StatsChangedObservable
                .Where(data => data.type == StatType.Ammo)
                .Subscribe(data => UpdateView(data.stat))
                .AddTo(this);   
            
            UpdateView(_entity.StatsController.GetNetworkStatValue(StatType.Ammo));
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient)
                return;
            
            _entity.Weapon.IsReloading.OnValueChanged -= OnReload;
        }

        private void UpdateView(NetworkStatValue stat)
        {
            bool isFillAmountMode = stat.Max > FillAmountModeThreshold;
            
            _ammoIndicatorWidth = _ammoIndicatorParent.rect.width / stat.Max;
            _indicatorModeObject.SetActive(!isFillAmountMode);
            _fillAmountModeObject.SetActive(isFillAmountMode);

            if (!isFillAmountMode)
            {
                foreach (Transform child in _ammoIndicatorParent)
                    Destroy(child.gameObject);

                for (int i = 0; i < stat.Current; i++)
                {
                    RectTransform indicator = Instantiate(_ammoIndicatorPrefab, _ammoIndicatorParent);
                    indicator.sizeDelta = new(_ammoIndicatorWidth, indicator.sizeDelta.y);
                }
            }
            else
                _ammoFillImage.fillAmount = stat.Current / stat.Max;
        }

        private void OnReload(bool isReloadingOld, bool isReloading)
        {
            if (isReloading)
            {
                _reloadingIndicator.transform.localEulerAngles = Vector3.zero;
                _reloadingTween = _reloadingIndicator.transform
                    .DOLocalRotate(new Vector3(0f, 0f, 360f), ReloadIconAnimationDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
            }
            else
            {
                _reloadingIndicator.transform.localEulerAngles = Vector3.zero;
                _reloadingTween?.Kill();
            }
        }
    }
}