namespace TankPlayground.Gameplay
{
    public class StatModifier
    {
        public readonly float Value;
        public readonly ModifierType Type;
        public readonly object Source;

        public StatModifier(ModifierType type, float value, object source = null)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}