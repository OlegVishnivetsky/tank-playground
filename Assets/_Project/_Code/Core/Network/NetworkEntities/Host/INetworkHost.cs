using Cysharp.Threading.Tasks;

namespace TankPlayground.Core.Network
{
    public interface INetworkHost
    {
        UniTask StartHostAsync(string lobbyName, int maxConnections, bool isPrivate);
    }
}