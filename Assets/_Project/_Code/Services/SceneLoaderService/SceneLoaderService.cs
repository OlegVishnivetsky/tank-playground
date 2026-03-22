using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TankPlayground.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLinq;

namespace TankPlayground.Services
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly LoadingScreen _loadingScreen;
        
        private readonly List<LoadingOperation> _preloadOperations = new();
        private readonly List<LoadingOperation> _pendingOperations = new();
        
        private UniTaskCompletionSource _readySource;
        
        private bool _isLoading;

        private const int LoadDelayMilliseconds = 200;

        public SceneLoaderService(LoadingScreen loadingScreen) => _loadingScreen = loadingScreen;

        public async UniTask LoadAsync(SceneName sceneName, bool networked = false)
        {
            try
            {
                if (_isLoading)
                {
                    Debug.LogWarning("Scene loading process is already in progress!!");
                    return;
                }

                _isLoading = true;
                _readySource = new();
                _loadingScreen.ResetAll();
                _loadingScreen.Show();
                _loadingScreen.SetStatus("Loading scene...");

                await ProcessLoadOperations(_preloadOperations);
                await LoadSceneAsync(sceneName, networked);
                await _readySource.Task;
                await ProcessLoadOperations(_pendingOperations);

                _loadingScreen.Hide();
                _isLoading = false;
            }
            catch (Exception e)
            {
                _isLoading = false;
                _loadingScreen.Hide();
                _readySource = null;
                _preloadOperations.Clear();
                _pendingOperations.Clear();
                
                Debug.LogError($"Error while loading scene: {e}");
            }
        }
        
        public void NotifyReady() => _readySource?.TrySetResult();

        public void AddPendingOperation(LoadingOperation operation) => _pendingOperations.Add(operation);
        
        public void AddPreloadOperation(LoadingOperation operation) => _preloadOperations.Add(operation);

        private async UniTask LoadSceneAsync(SceneName sceneName, bool networked = false)
        {
            if (networked)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);
                UniTaskCompletionSource tcs = new UniTaskCompletionSource();
        
                void OnLoadComplete(ulong u, string s, LoadSceneMode m)
                {
                    NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
                    tcs.TrySetResult();
                }

                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
                await tcs.Task;
            }
            else
            {
                await SceneManager.LoadSceneAsync(sceneName.ToString());
            }
        }
        
        private async UniTask ProcessLoadOperations(List<LoadingOperation> operations)
        {
            try
            {
                float totalWeight = operations
                    .AsValueEnumerable()
                    .Sum(op => op.Weight);
                float completedWeight = 0f;

                while (operations.Count > 0)
                {
                    LoadingOperation operation = operations[0];
                
                    operations.RemoveAt(0);
                    _loadingScreen.SetStatus(operation.Description);

                    float capturedCompleted = completedWeight;
                    float opWeight = operation.Weight;

                    Progress<float> progress = new Progress<float>(p =>
                    {
                        float total = (capturedCompleted + p * opWeight) / totalWeight;
                        _loadingScreen.SetProgress(total);
                    });

                    await operation.Execute(progress);
                    completedWeight += opWeight;
                }

                _loadingScreen.SetProgress(1f);
    
                await UniTask.WaitUntil(() => _loadingScreen.FillAmount >= 0.99f);
                await UniTask.Delay(LoadDelayMilliseconds);
            }
            catch (Exception e)
            {
                _isLoading = false;
                _loadingScreen.Hide();
                _readySource = null;
                _preloadOperations.Clear();
                _pendingOperations.Clear();
                
                Debug.LogWarning($"Error while loading pending operations: {e}");
            }
        }
    }
}