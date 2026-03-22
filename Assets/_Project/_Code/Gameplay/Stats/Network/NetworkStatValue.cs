using Unity.Netcode;

namespace TankPlayground.Gameplay
{
    public struct NetworkStatValue : INetworkSerializable
    {
        public float Max;
        public float Current;

        public NetworkStatValue(float max, float current)
        {
            Max = max;
            Current = current;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Max);
            serializer.SerializeValue(ref Current);
        }
    }
}