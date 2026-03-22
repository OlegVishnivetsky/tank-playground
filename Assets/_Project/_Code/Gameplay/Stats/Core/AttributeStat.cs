using System.Collections.Generic;
using R3;

namespace TankPlayground.Gameplay
{
    public class AttributeStat : IStat
    {
        private readonly List<StatModifier> _modifiers = new();
        
        private float _baseValue;
        private float _cachedValue;
        private bool _isDirty = true;
        
        public StatType Type { get; }
        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _isDirty = false;
                    CalculateFinal();
                }
                
                return _cachedValue;
            }
        }
        
        private readonly Subject<float> _changedSubject = new();
        public Observable<float> ChangedObservable => _changedSubject;

        public AttributeStat(StatType type, float baseValue)
        {
            Type = type;
            _baseValue = baseValue;
        }

        public void SetValue(float value)
        {
            _baseValue = value;
            _changedSubject.OnNext(Value);
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            _isDirty = true;
            _changedSubject.OnNext(Value);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            bool isRemoved = _modifiers.Remove(modifier);
            _isDirty = isRemoved;
            
            if (!isRemoved)
                return;
            
            _changedSubject.OnNext(Value);
        }

        public void RemoveModifiersFromSource(object source)
        {
            int removed = _modifiers.RemoveAll(m => m.Source == source);
            
            if (removed > 0)
            {
                _isDirty = true;
                _changedSubject.OnNext(Value);
            }
        }

        private void CalculateFinal()
        {
            float flat = _baseValue;
            float percentAdd = 0f;
            float percentMult = 1f;

            foreach (StatModifier modifier in _modifiers)
            {
                switch (modifier.Type)
                {
                    case ModifierType.Flat:
                        flat += modifier.Value;
                        break;
                    case ModifierType.PercentAdd:
                        percentAdd += modifier.Value;
                        break;
                    case ModifierType.Multiplier:
                        percentMult *= 1f + modifier.Value;
                        break;
                    case ModifierType.Override:
                        _cachedValue = modifier.Value;
                        return;
                }
            }

            _cachedValue = flat * (1f + percentAdd) * percentMult;
            _isDirty = false;
        }
    }
}