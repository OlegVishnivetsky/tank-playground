using System;
using UnityEngine;

namespace TankPlayground.UI
{
    [Serializable]
    public class ComponentToggleStrategy : BaseToggleStrategy
    {
        [SerializeField] private MonoBehaviour _component;
        
        public override void UpdateVisualState(bool enabled) => _component.enabled = enabled;
    }
}