using System.Collections.Generic;
using R3;

namespace TankPlayground.Gameplay
{
    public interface IStatusEffectService
    {
        Observable<List<StatusEffect>> EffectsObservable { get; }

        StatusEffect GetEffect(string name);
        void AddEffect(StatusEffect effect);
        void Tick(float deltaTime);
        void RemoveEffect(StatusEffect effect);
    }
}