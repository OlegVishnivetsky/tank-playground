using System;
using TMPro;
using UnityEngine;

namespace TankPlayground.UI
{
    [Serializable]
    public class TextToggleStrategy : BaseToggleStrategy
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _enabledText;
        [SerializeField] private string _disabledText;
        
        public override void UpdateVisualState(bool enabled) => 
            _text.text = enabled ? _enabledText : _disabledText;
    }
}