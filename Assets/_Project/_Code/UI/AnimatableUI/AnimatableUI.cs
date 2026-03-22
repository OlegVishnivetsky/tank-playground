using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TankPlayground.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AnimatableUI : MonoBehaviour
    {
        [FoldoutGroup("Animatable UI")]
        [SerializeField] private float _animationDuration = 0.5f;
        [FoldoutGroup("Animatable UI")]
        [SerializeField] private bool _hideAtStart = true;

        [Title("Strategies")] 
        [FoldoutGroup("Animatable UI")] 
        [SerializeReference]
        private List<BaseAnimationStrategy> _animationStrategies = new()
        {
            new FadeAnimationStrategy()
        };
        
        private CanvasGroup _canvasGroup;
        private Sequence _currentAnimation;
        private RectTransform _rectTransform;
        
        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            
            foreach (BaseAnimationStrategy strategy in _animationStrategies)
                strategy.Setup(_rectTransform, _canvasGroup);
        }
        
        protected virtual void Start()
        {
            if (_hideAtStart)
                ResetToHiddenState();
        }
        
        public virtual void Show(float delay = 0f)
        {
            _currentAnimation?.Kill();
            _currentAnimation = DOTween.Sequence();
            _currentAnimation.SetDelay(delay);
            
            foreach (BaseAnimationStrategy strategy in _animationStrategies)
                _currentAnimation.Join(strategy.PerformShowAnimation(_animationDuration));
            
            _currentAnimation.OnComplete(() => _canvasGroup.blocksRaycasts = true);
            _currentAnimation.Play();
        }
        
        public virtual void Hide(float delay = 0f)
        {
            _canvasGroup.blocksRaycasts = false;
            _currentAnimation?.Kill();
            _currentAnimation = DOTween.Sequence();
            _currentAnimation.SetDelay(delay);
            
            foreach (BaseAnimationStrategy strategy in _animationStrategies) 
                _currentAnimation.Join(strategy.PerformHideAnimation(_animationDuration));
            
            _currentAnimation.Play();
        }

        public void ResetToHiddenState()
        {
            _canvasGroup.blocksRaycasts = false;
            
            foreach (BaseAnimationStrategy strategy in _animationStrategies)
                strategy.Prepare();
        }
    }
}