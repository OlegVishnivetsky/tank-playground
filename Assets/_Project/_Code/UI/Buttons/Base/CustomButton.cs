using DG.Tweening;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace TankPlayground.UI
{
    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool _isInteractable = true;
        [SerializeField] private bool _useSound = true;

        [ShowIf("_useSound")] 
        [SerializeField] private string _soundName = "Click 1";

        [Title("Throttle Settings")]
        [SerializeField] private bool _useThrottle;
        [ShowIf("_useThrottle")]
        [SerializeField] private float _throttleDuration = 1f;
        
        [Title("Animation")] 
        [FoldoutGroup("Animation")] 
        [SerializeField] private bool _useColorAnimation = true;
        [FoldoutGroup("Animation")]
        [SerializeField] private bool _useClickAnimation = true;
        [FoldoutGroup("Animation")]
        [SerializeField] private float _clickAnimationDuration = 0.09f;
        [FoldoutGroup("Animation")]
        [SerializeField] private float _disabledAlphaValue = 0.4f;

        [FoldoutGroup("Animation")]
        [SerializeField] private Ease _scaleDownEase = Ease.OutSine;
        [FoldoutGroup("Animation")]
        [SerializeField] private Ease _scaleUpEase = Ease.OutSine;

        [FoldoutGroup("Animation")]
        [Title("Advanced Settings")] 
        [SerializeField] private float _rotationAngle = -5f;

        //private ISoundService _soundService;
        
        private float _lastClickTime;
        private Color _originalColor;

        public Image Image { get; private set; }
        public TextMeshProUGUI Text { get; private set; }
        public CanvasGroup CanvasGroup { get; set; }
        
        public bool IsInteractable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;
                UpdateInteractionView();
            }
        }

        private readonly Subject<Unit> _clickedSubject = new();
        public Observable<Unit> ClickedObservable => _clickedSubject;

        private const float ScaleDownFactor = 0.85f;
        private const string DefaultSoundName = "Click 1";

        // [Inject]
        // public void Construct(ISoundService soundService) => _soundService = soundService;
        
        protected virtual void Awake()
        {
            Image = GetComponent<Image>();
            Text = GetComponentInChildren<TextMeshProUGUI>();
            CanvasGroup = GetComponent<CanvasGroup>();

            if (string.IsNullOrEmpty(_soundName))
                _soundName = DefaultSoundName;
            
            if (Image != null)
                _originalColor = Image.color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isInteractable) 
                return;

            if (_useClickAnimation)
            {
                AnimateScale(ScaleDownFactor, _scaleDownEase);
                AnimateRotation(_rotationAngle);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isInteractable)
                return;

            if (_useClickAnimation)
            {
                AnimateScale(1f, _scaleUpEase, true);
                AnimateRotation(0f);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isInteractable) 
                return;

            if (_useThrottle)
            {
                float timeSinceLastClick = Time.time - _lastClickTime;
                
                if (timeSinceLastClick < _throttleDuration)
                    return;

                _lastClickTime = Time.time;
            }
            
            // if (_useSound)
            //     _soundService.PlaySound(_soundName);
            
            OnClicked();
        }

        private void AnimateScale(float targetScale, Ease ease, bool invokeClick = false) =>
            transform
                .DOScale(targetScale, _clickAnimationDuration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    if (invokeClick)
                        PlayBounceAnimation();
                });

        private void AnimateRotation(float targetAngle) =>
            transform
                .DORotate(new Vector3(0, 0, targetAngle), _clickAnimationDuration)
                .SetEase(_scaleDownEase);

        private void AnimateColor(Color targetColor)
        {
            if (!_useColorAnimation)
                return;
            
            if (Image == null)
                return;
            
            Image
                .DOColor(targetColor, _clickAnimationDuration)
                .SetEase(_scaleDownEase);
        }
        
        private void PlayBounceAnimation()
        {
            const float bounceScale = 1.1f;
            const float bounceDuration = 0.1f;

            transform
                .DOScale(bounceScale, bounceDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    transform
                        .DOScale(1f, bounceDuration)
                        .SetEase(Ease.InQuad);
                });
        }

        private void UpdateInteractionView()
        {
            if (Image == null)
                return;
            
            Image.DOFade(_isInteractable ? 1 : _disabledAlphaValue, 0f);
        }

        public virtual void OnClicked() => _clickedSubject.OnNext(Unit.Default);
    }
}