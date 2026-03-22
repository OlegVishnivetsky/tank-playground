using R3;

namespace TankPlayground.Gameplay
{
    public interface IStat
    {
        StatType Type { get; }
        float Value { get; }
        Observable<float> ChangedObservable { get; }

        void SetValue(float value);
    }
}