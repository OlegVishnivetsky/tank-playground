using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TankPlayground.Services;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace TankPlayground.Core.Network
{
    public class NetworkHost : INetworkHost, IDisposable
    {
        private readonly ISceneLoaderService _sceneLoaderService;
        
        private Allocation _allocation;
        private CancellationTokenSource _heartbeatCancellation;
        
        private string _lobbyId;
        private string _joinCode;

        public NetworkHost(ISceneLoaderService sceneLoaderService) => _sceneLoaderService = sceneLoaderService;

        private const float LobbyHeartbeatInterval = 15f;
        private const string ConnectionType = "udp";

        public void Dispose()
        {
            _heartbeatCancellation?.Cancel();
            _heartbeatCancellation?.Dispose();
            _heartbeatCancellation = null;
        }

        public async UniTask StartHostAsync(string lobbyName, int maxConnections, bool isPrivate)
        {
            try
            {
                _sceneLoaderService.AddPreloadOperation(new("Setting up...", 30f, async _ =>
                {
                    _allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
                    _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                }));
                
                _sceneLoaderService.AddPreloadOperation(new("Creating lobby...", 30f, async _ =>
                {
                    CreateLobbyOptions lobbyOptions = new()
                    {
                        IsPrivate = isPrivate,
                        Data = new()
                        {
                            {
                                "JoinCode", new(DataObject.VisibilityOptions.Member, _joinCode)
                            }
                        }
                    };
                    Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxConnections, lobbyOptions);
                    _lobbyId = lobby.Id;

                    Debug.Log($"JOIN CODE: {_joinCode}");
                    
                    _heartbeatCancellation = new();
                    StartLobbyHeartbeatAsync().Forget();
                }));

                _sceneLoaderService.AddPreloadOperation(new("Connecting...", 10f, async _ =>
                {
                    RelayServerData serverData = new(_allocation, ConnectionType);
                    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                    transport.SetRelayServerData(serverData);
                }));

                await _sceneLoaderService.LoadAsync(SceneName.Gameplay);
                
                NetworkManager.Singleton.StartHost();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error while starting host: {e}");
            }
        }

        private async UniTaskVoid StartLobbyHeartbeatAsync()
        {
            try
            {
                while (!_heartbeatCancellation.IsCancellationRequested)
                {
                    if (_heartbeatCancellation.IsCancellationRequested)
                        break;
                    
                    await Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
                    await UniTask.WaitForSeconds(LobbyHeartbeatInterval, cancellationToken: _heartbeatCancellation.Token);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error while starting lobby heartbeat: {e}");
            }
        }
    }
}