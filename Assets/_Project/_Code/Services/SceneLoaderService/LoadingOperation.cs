using System;
using Cysharp.Threading.Tasks;

namespace TankPlayground.Services
{
    public class LoadingOperation
    {
        public string Description { get; }
        public float Weight { get; }
        public Func<IProgress<float>, UniTask> Execute { get; }

        public LoadingOperation(string description, float weight, Func<IProgress<float>, UniTask> execute)
        {
            Description = description;
            Weight = weight;
            Execute = execute;
        }
    }
}