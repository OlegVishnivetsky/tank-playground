using System;
using System.Collections.Generic;
using ZLinq;

namespace TankPlayground.Gameplay
{
    public class StatCollection : IStatCollection
    {
        private readonly Dictionary<StatType, IStat> _stats = new();
        private readonly Dictionary<StatType, ResourceStat> _resourceStats = new();

        public IReadOnlyList<IStat> Attributes => _stats.Values.AsValueEnumerable().ToList();
        public IReadOnlyList<ResourceStat> Resources => _resourceStats.Values.AsValueEnumerable().ToList();
        
        public StatCollection(List<IStat> stats)
        {
            foreach (IStat stat in stats)
                Register(stat);
        }

        public StatCollection(params IStat[] stats)
        {
            foreach (IStat stat in stats)
                Register(stat);
        }

        public void RegisterResource(ResourceStat stat) => _resourceStats[stat.Type] = stat;

        public void RegisterResources(params ResourceStat[] stats)
        {
            foreach (ResourceStat resourceStat in stats)
                _resourceStats[resourceStat.Type] = resourceStat;
        }

        public void Register(IStat stat) => _stats[stat.Type] = stat;
        
        public void Register(params IStat[] stats)
        {
            foreach (IStat stat in stats)
                _stats[stat.Type] = stat;
        }

        public IStat Get(StatType type) => _stats.GetValueOrDefault(type);

        public float GetValue(StatType type) => Get(type).Value;
        
        public bool TryGet<T>(StatType type, out T stat) where T : class, IStat
        {
            if (_stats.TryGetValue(type, out IStat raw))
            {
                stat = raw as T;
                return stat != null;
            }

            stat = null;
            return false;
        }

        public bool TryGetResource(StatType type, out ResourceStat stat)
        {
            if (_resourceStats.TryGetValue(type, out ResourceStat resourceStat))
            {
                stat = resourceStat;
                return stat != null;
            }

            stat = null;
            return false;
        }
    }
}