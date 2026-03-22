using DG.Tweening;
using UnityEngine;

namespace TankPlayground.UI
{
    public interface IAnimationStrategy
    {
        void Setup(RectTransform transform, CanvasGroup canvasGroup);
        void Prepare();
        Tween PerformShowAnimation(float duration);
        Tween PerformHideAnimation(float duration);
    }
}