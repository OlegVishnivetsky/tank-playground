using System;
using Unity.Collections;
using Unity.Netcode;

namespace TankPlayground.Gameplay
{
    public struct NetworkStatusEffect : INetworkSerializable, IEquatable<NetworkStatusEffect>
    {
        public FixedString64Bytes Name;
        public float Duration;
        public int Stacks;

        public NetworkStatusEffect(FixedString64Bytes name, float duration, int stacks)
        {
            Name = name;
            Duration = duration;
            Stacks = stacks;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Duration);
            serializer.SerializeValue(ref Stacks);
        }
        
        public bool Equals(NetworkStatusEffect other) => Name.Equals(other.Name);
    }
}