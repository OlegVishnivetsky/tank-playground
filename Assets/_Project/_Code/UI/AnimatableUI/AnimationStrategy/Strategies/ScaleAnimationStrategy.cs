using System;
using DG.Tweening;
using UnityEngine;

namespace TankPlayground.UI
{
    [Serializable]
    public class ScaleAnimationStrategy : BaseAnimationStrategy
    {
        private Vector3 _originalScale;
        
        public override void Setup(RectTransform transform, CanvasGroup canvasGroup)
        {
            base.Setup(transform, canvasGroup);
            _originalScale = transform.localScale;

            if (_originalScale == Vector3.zero)
                _originalScale = Vector3.one;
        }

        public override void Prepare() => Transform.localScale = Vector3.zero;

        public override Tween PerformShowAnimation(float duration) => 
            Transform.DOScale(_originalScale, duration).SetEase(EasingCurve);

        public override Tween PerformHideAnimation(float duration) => 
            Transform.DOScale(Vector3.zero, duration).SetEase(EasingCurve);
    }
}