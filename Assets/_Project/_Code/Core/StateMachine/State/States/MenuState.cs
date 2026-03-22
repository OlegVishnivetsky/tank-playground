using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TankPlayground.Configs;
using TankPlayground.Services;
using TankPlayground.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Screen = TankPlayground.UI.Screen;

namespace TankPlayground.Core.StateMachine
{
    public class MenuState : IEnterState, IExitState
    {
        private readonly ISceneLoaderService _sceneLoaderService;
        private readonly IUINavigationService _navigationService;
        private readonly IAssetProviderService _assetProvider;
        private readonly UIScreenConfig _screenConfig;
        
        private AssetScope _menuAssetScope;
        private CancellationTokenSource _cancellation;

        public MenuState(
            ISceneLoaderService sceneLoaderService,
            IUINavigationService navigationService,
            IAssetProviderService assetProvider,
            UIScreenConfig screenConfig)
        {
            _sceneLoaderService = sceneLoaderService;
            _navigationService = navigationService;
            _assetProvider = assetProvider;
            _screenConfig = screenConfig;
        }

        public void Enter()
        {
            _menuAssetScope = new(_assetProvider);
            _cancellation = new CancellationTokenSource();
            _sceneLoaderService.AddPendingOperation(new("Load UI...", 
                15f, async _ => InitializeAsync(_cancellation.Token).Forget()));
            _sceneLoaderService.NotifyReady();
        }

        public void Exit()
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _menuAssetScope?.Dispose();
        }

        private async UniTask InitializeAsync(CancellationToken cancellation)
        {
            try
            {
                List<Screen> screenInstances = new();
                List<AssetReferenceGameObject> screenReferences = _screenConfig.GetAll(
                    UIScreenType.MainMenuMain,
                    UIScreenType.MainMenuSettings,
                    UIScreenType.MainMenuJoin,
                    UIScreenType.MainMenuLobbies,
                    UIScreenType.MainMenuCreateGame);

                foreach (AssetReferenceGameObject reference in screenReferences)
                {
                    cancellation.ThrowIfCancellationRequested();
                    Screen screenInstance = await _menuAssetScope.InstantiateAsync<Screen>(reference);
                    screenInstances.Add(screenInstance);
                }

                cancellation.ThrowIfCancellationRequested();
                _navigationService.Initialize(screenInstances);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.LogError($"Error while initializing menu: {e}");
            }
        }
    }
}