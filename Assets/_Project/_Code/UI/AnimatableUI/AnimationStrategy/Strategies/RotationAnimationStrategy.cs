using System;
using DG.Tweening;
using UnityEngine;

namespace TankPlayground.UI
{
    [Serializable]
    public class RotationAnimationStrategy : BaseAnimationStrategy
    {
        [SerializeField] private Vector3 _hiddenRotation = new(0, 0, 90);
        
        private Vector3 _originalRotation;
        
        public override void Setup(RectTransform transform, CanvasGroup canvasGroup)
        {
            base.Setup(transform, canvasGroup);
            _originalRotation = transform.localEulerAngles;
        }
        
        public override void Prepare() => Transform.localEulerAngles = _hiddenRotation;

        public override Tween PerformShowAnimation(float duration) => 
            Transform.DOLocalRotate(_originalRotation, duration).SetEase(EasingCurve);

        public override Tween PerformHideAnimation(float duration) => 
            Transform.DOLocalRotate(_hiddenRotation, duration).SetEase(EasingCurve);
    }
}