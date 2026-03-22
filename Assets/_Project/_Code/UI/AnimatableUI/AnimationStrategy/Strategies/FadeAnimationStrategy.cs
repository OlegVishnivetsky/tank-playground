using System;
using DG.Tweening;

namespace TankPlayground.UI
{
    [Serializable]
    public class FadeAnimationStrategy : BaseAnimationStrategy
    {
        public override void Prepare() => CanvasGroup.alpha = 0f;

        public override Tween PerformShowAnimation(float duration) => 
            CanvasGroup.DOFade(1f, duration).SetEase(EasingCurve);

        public override Tween PerformHideAnimation(float duration) =>
            CanvasGroup.DOFade(0f, duration).SetEase(EasingCurve);
    }
}