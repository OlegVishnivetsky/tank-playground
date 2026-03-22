using System.Collections.Generic;
using R3;
using ZLinq;

namespace TankPlayground.Gameplay
{
    public class StatusEffectService : IStatusEffectService
    {
        private readonly IStatCollection _stats;
        private readonly List<StatusEffect> _effects = new();
        
        public IReadOnlyList<StatusEffect> ActiveEffects => _effects;
        
        private readonly Subject<List<StatusEffect>> _effectsSubject = new();
        public Observable<List<StatusEffect>> EffectsObservable => _effectsSubject;
        
        public StatusEffectService(IStatCollection stats) => _stats = stats;

        public StatusEffect GetEffect(string name) => 
            _effects
                .AsValueEnumerable()
                .FirstOrDefault(e => e.Config.Name == name);
        
        public void AddEffect(StatusEffect effect)
        {
            StatusEffect existing = _effects.Find(e => e.Config == effect.Config);
    
            if (existing != null)
            {
                switch (effect.Config.Stacking)
                {
                    case StackingBehaviourType.None:
                        return;
                
                    case StackingBehaviourType.Stack:
                        existing.AddStack();
                        existing.ResetDuration();
                        return;
                
                    case StackingBehaviourType.StackDuration:
                        existing.ExtendDuration(effect.Config.Duration);
                        return;
                }
            }
    
            _effects.Add(effect);
            effect.Apply(_stats);
            _effectsSubject.OnNext(_effects);
        }

        public void Tick(float deltaTime)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                _effects[i].Tick(_stats, deltaTime);
                
                if (_effects[i].RemainingDuration <= 0 && _effects[i].Config.Duration > 0)
                {
                    _effects[i].Remove(_stats);
                    _effects.RemoveAt(i);
                }
            }
        }

        public void RemoveEffect(StatusEffect effect)
        {
            if (_effects.Remove(effect))
            {
                effect.Remove(_stats);
                _effectsSubject.OnNext(_effects);
            }
        }
    }
}