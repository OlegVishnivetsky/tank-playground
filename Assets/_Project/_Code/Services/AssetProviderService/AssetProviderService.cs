using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace TankPlayground.Services
{
    public class AssetProviderService : IAssetProviderService
    {
        private readonly DiContainer _container;
        private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

        private SceneInstance _sceneHandle;

        public AssetProviderService(DiContainer container) => _container = container;
        
        public async UniTask<T> InstantiateAsync<T>(AssetReference reference, Transform parent = null) where T : Object
        {
            try
            {
                string key = reference.AssetGUID;

                if (!_handles.TryGetValue(key, out AsyncOperationHandle existing))
                {
                    AsyncOperationHandle<GameObject> operation = Addressables.LoadAssetAsync<GameObject>(reference);
                    _handles.Add(key, operation);
                    await operation.Task;
                }

                AsyncOperationHandle<GameObject> instantiateHandle = Addressables.InstantiateAsync(reference, parent);
                GameObject instance = await instantiateHandle.Task;
                T component = instance.GetComponent<T>();

                _container.InjectGameObject(instance);
                
                if (component == null)
                {
                    Debug.LogError($"Component {typeof(T).Name} not found on {instance.name}");
                    Addressables.ReleaseInstance(instance);
                    return null;
                }

                return component;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async UniTask<T> LoadAsync<T>(AssetReference reference) where T : Object
        {
            try
            {
                string key = reference.AssetGUID;

                if (_handles.TryGetValue(key, out AsyncOperationHandle existing))
                    return existing.Result as T;
                
                AsyncOperationHandle<T> operation = Addressables.LoadAssetAsync<T>(reference);
                
                _handles.Add(reference.AssetGUID, operation);
                
                return await operation.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading asset: {e}");
                return null;
            }
        }

        public void Release(AssetReference reference)
        {
            string key = reference.AssetGUID;

            if (_handles.Remove(key, out AsyncOperationHandle handle))
            {
                Debug.Log($"Releasing asset: {key}");
                Addressables.Release(handle);
            }
        }

        public void Release(GameObject gameObject) => Addressables.Release(gameObject);

        public void ReleaseAll()
        {
            foreach (AsyncOperationHandle handle in _handles.Values)
                Addressables.Release(handle);

            _handles.Clear();
        }
    }
}