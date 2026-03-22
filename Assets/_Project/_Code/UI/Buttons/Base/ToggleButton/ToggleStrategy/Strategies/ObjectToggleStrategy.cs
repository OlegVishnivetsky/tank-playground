using System;
using UnityEngine;

namespace TankPlayground.UI
{
    [Serializable]
    public class ObjectToggleStrategy : BaseToggleStrategy
    {
        [SerializeField] private GameObject _gameObject;
        
        public override void UpdateVisualState(bool enabled) => _gameObject.SetActive(enabled);
    }
}