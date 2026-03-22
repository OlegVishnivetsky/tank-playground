using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace TankPlayground.Core.Network
{
    public interface INetworkClient
    {
        UniTask AuthenticateAsync();
        UniTask<bool> TryJoinPublicLobbyAsync(Lobby lobby);
        UniTask<bool> TryJoinGameAsync(string joinCode);
    }
}