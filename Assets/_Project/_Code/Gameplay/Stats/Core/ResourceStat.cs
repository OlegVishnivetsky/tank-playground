using System;
using R3;

namespace TankPlayground.Gameplay
{
    public class ResourceStat : IStat
    {
        private readonly AttributeStat _maxStat;
        private ReactiveProperty<float> _current = new();
        
        public StatType Type { get; }
        public float Value { get; }

        public ReadOnlyReactiveProperty<float> Current => _current;
        public float Max => _maxStat.Value;
        
        private readonly Subject<float> _changedSubject = new();
        public Observable<float> ChangedObservable => _changedSubject;
        
        public ResourceStat(StatType type, float baseValue, bool setToMax = true)
        {
            Type = type;
            Value = baseValue;
            _maxStat = new AttributeStat(type, baseValue);
            
            if (setToMax)
                _current = new(_maxStat.Value);
            
            _maxStat.ChangedObservable.Subscribe(OnMaxChanged);
        }

        private void OnMaxChanged(float newMax) => _current.Value = Math.Min(_current.Value, newMax);

        public void SetValue(float value)
        {
            _current.Value = value;
            _changedSubject.OnNext(_current.Value);
        }
        
        public void Decrease() => Modify(-1);
        
        public void Increase() => Modify(1);

        public void Restore()
        {
            _current.Value = Max;
            _changedSubject.OnNext(_current.Value);
        }
        
        public void Modify(float amount)
        {
            _current.Value = Math.Clamp(_current.Value + amount, 0, Max);
            _changedSubject.OnNext(_current.Value);
        }
    }
}