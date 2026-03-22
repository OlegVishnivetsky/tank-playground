using System;
using Cysharp.Threading.Tasks;
using TankPlayground.Core.Network;
using TankPlayground.Services;
using UnityEngine;
using UnityEngine.Rendering;

namespace TankPlayground.Core.StateMachine
{
    public class BootState : IEnterState
    {
        private readonly INetworkClient _networkClient;
        private readonly ISceneLoaderService _sceneLoaderService;
        
        public BootState(
            INetworkClient networkClient,
            ISceneLoaderService sceneLoaderService)
        {
            _networkClient = networkClient;
            _sceneLoaderService = sceneLoaderService;
        }

        public void Enter() => BootAsync().Forget();

        private async UniTaskVoid BootAsync()
        {
            try
            {
                bool isDedicatedServer = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
            
                if (isDedicatedServer)
                    return;
            
                _sceneLoaderService.AddPreloadOperation(new("Authenticating...", 15f, 
                    async _ => await _networkClient.AuthenticateAsync()));
                
                await _sceneLoaderService.LoadAsync(SceneName.Menu);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while booting: {e}");
            }
        }
    }
}