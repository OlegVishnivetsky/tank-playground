using System;
using UnityEngine;
using UnityEngine.UI;

namespace TankPlayground.UI
{
    [Serializable]
    public class SpriteToggleStrategy : BaseToggleStrategy
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _enabledSprite;
        [SerializeField] private Sprite _disabledSprite;
        
        public override void UpdateVisualState(bool enabled) =>
            _image.sprite = enabled ? _enabledSprite : _disabledSprite;
    }
}