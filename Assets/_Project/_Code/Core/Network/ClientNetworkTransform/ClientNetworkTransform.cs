using Unity.Netcode.Components;

namespace TankPlayground.Core.Network
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override void Update()
        {
            CanCommitToTransform = IsOwner;

            base.Update();

            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                if (!CanCommitToTransform)
                    return;

                TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }

        protected override bool OnIsServerAuthoritative() => false;
    }
}