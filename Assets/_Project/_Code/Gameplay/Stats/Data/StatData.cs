using System;
using TankPlayground.Gameplay;

namespace TankPlayground.Config
{
    [Serializable]
    public class StatData
    {
        public StatType Type;
        public float Value;
        public bool SetToMax = true;
        
        public StatData(StatType type, float value, bool setToMax = true)
        {
            Type = type;
            Value = value;
            SetToMax = setToMax;
        }
    }
}