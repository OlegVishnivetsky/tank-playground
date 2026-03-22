using System;
using DG.Tweening;
using UnityEngine;

namespace TankPlayground.UI
{
    [Serializable]
    public class PositionAnimationStrategy : BaseAnimationStrategy
    {
        [SerializeField] private AnimationDirection _direction = AnimationDirection.Right;
        [SerializeField] private float _distance = 100f;
        
        private Vector2 _originalPosition;
        private Vector3 _hiddenPosition;

        public override void Setup(RectTransform transform, CanvasGroup canvasGroup)
        {
            base.Setup(transform, canvasGroup);
            _originalPosition = transform.anchoredPosition;
            CalculateHiddenPosition();
        }

        public override void Prepare() => Transform.anchoredPosition = _hiddenPosition;

        public override Tween PerformShowAnimation(float duration) => 
            Transform.DOAnchorPos(_originalPosition, duration).SetEase(EasingCurve);

        public override Tween PerformHideAnimation(float duration) => 
            Transform.DOAnchorPos(_hiddenPosition, duration).SetEase(EasingCurve);

        private void CalculateHiddenPosition()
        {
            _hiddenPosition = _originalPosition;
            
            switch (_direction)
            {
                case AnimationDirection.Left:
                    _hiddenPosition += Vector3.left * _distance;
                    break;
                case AnimationDirection.Right:
                    _hiddenPosition += Vector3.right * _distance;
                    break;
                case AnimationDirection.Top:
                    _hiddenPosition += Vector3.up * _distance;
                    break;
                case AnimationDirection.Bottom:
                    _hiddenPosition += Vector3.down * _distance;
                    break;
            }
        }
    }
}