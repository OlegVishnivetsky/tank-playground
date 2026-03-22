using System;
using UnityEngine;
using UnityEngine.UI;

namespace TankPlayground.UI
{
    [Serializable]
    public class ColorToggleStrategy : BaseToggleStrategy
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _enabledColor;
        [SerializeField] private Color _disabledColor;
        
        public override void UpdateVisualState(bool enabled) => 
            _image.color = enabled ? _enabledColor : _disabledColor;
    }
}