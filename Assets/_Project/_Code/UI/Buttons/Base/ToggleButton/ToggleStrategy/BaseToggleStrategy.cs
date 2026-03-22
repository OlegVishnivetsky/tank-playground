using System;

namespace TankPlayground.UI
{
    [Serializable]
    public abstract class BaseToggleStrategy : IToggleStrategy
    {
        protected CustomButton Button;

        public void Initialize(CustomButton button) => Button = button;
        
        public abstract void UpdateVisualState(bool enabled);
    }
}