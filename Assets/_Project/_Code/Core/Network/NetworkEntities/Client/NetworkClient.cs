using System;
using Cysharp.Threading.Tasks;
using TankPlayground.Services;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace TankPlayground.Core.Network
{
    public class NetworkClient : INetworkClient
    {
        private readonly IAuthService _authService;
        private readonly ISceneLoaderService _sceneLoaderService;

        private JoinAllocation _joinAllocation;
        
        private const string ConnectionType = "udp";
        
        public NetworkClient(
            IAuthService authService,
            ISceneLoaderService sceneLoaderService)
        {
            _authService = authService;
            _sceneLoaderService = sceneLoaderService;
        }

        public async UniTask AuthenticateAsync()
        {
            await UnityServices.InitializeAsync();
            await _authService.AuthenticateAnonymouslyAsync();
        }

        public async UniTask<bool> TryJoinPublicLobbyAsync(Lobby lobby)
        {
            try
            {
                Lobby lobbyToJoin = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                string joinCode = lobbyToJoin.Data["JoinCode"].Value;
                
                return await TryJoinGameAsync(joinCode);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error while joining Public lobby: {e}");
                return false;
            }
        }
        
        public async UniTask<bool> TryJoinGameAsync(string joinCode)
        {
            try
            {
                _joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
                _sceneLoaderService.AddPreloadOperation(new("Joining game...", 15f, async _ =>
                {
                    RelayServerData serverData = new(_joinAllocation, ConnectionType);
                    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                    transport.SetRelayServerData(serverData);
                }));
            
                await _sceneLoaderService.LoadAsync(SceneName.Gameplay);
                
                NetworkManager.Singleton.StartClient();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error while joining game: {e}");
                return false;
            }
        }
    }
}