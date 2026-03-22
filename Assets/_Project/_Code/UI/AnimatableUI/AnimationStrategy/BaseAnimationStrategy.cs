using System;
using DG.Tweening;
using UnityEngine;

namespace TankPlayground.UI
{
    [Serializable]
    public abstract class BaseAnimationStrategy : IAnimationStrategy
    {
        [SerializeField] protected AnimationCurve EasingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        protected RectTransform Transform;
        protected CanvasGroup CanvasGroup;
        
        public virtual void Setup(RectTransform transform, CanvasGroup canvasGroup)
        {
            Transform = transform;
            CanvasGroup = canvasGroup;
        }

        public abstract void Prepare();
        public abstract Tween PerformShowAnimation(float duration);
        public abstract Tween PerformHideAnimation(float duration);
    }
}