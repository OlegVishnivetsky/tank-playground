using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankPlayground.Gameplay.UI
{
    public class StatusEffectItemView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _stacksText;
        [SerializeField] private Image _cooldownFillImage;

        public void Initialize(NetworkStatusEffect effect, Sprite icon)
        {
            float time = effect.Duration;
            
            _iconImage.sprite = icon;
            _stacksText.text = effect.Stacks.ToString();
            _stacksText.gameObject.SetActive(effect.Stacks > 1);

            DOTween
                .To(() => time, x => time = x, 0, effect.Duration)
                .SetEase(Ease.Linear)
                .OnUpdate(() => _cooldownFillImage.fillAmount = time / effect.Duration)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}