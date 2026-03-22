using System.Collections.Generic;

namespace TankPlayground.Gameplay
{
    public interface IStatCollection
    {
        IReadOnlyList<IStat> Attributes { get; }
        IReadOnlyList<ResourceStat> Resources { get; }
        
        void RegisterResource(ResourceStat stat);
        void RegisterResources(params ResourceStat[] stats);
        void Register(IStat stat);
        void Register(params IStat[] stats);
        IStat Get(StatType type);
        float GetValue(StatType type);
        bool TryGet<T>(StatType type, out T stat) where T : class, IStat;
        bool TryGetResource(StatType type, out ResourceStat stat);
    }
}