using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TankPlayground.Services
{
    public class AssetScope : IDisposable
    {
        private readonly IAssetProviderService _assetProvider;
        private readonly List<AssetReference> _references = new();
        
        public AssetScope(IAssetProviderService assetProvider) => _assetProvider = assetProvider;

        public void Dispose()
        {
            foreach (AssetReference reference in _references)
                _assetProvider.Release(reference);
            
            _references.Clear();
        }

        public async UniTask<T> LoadAsync<T>(AssetReference reference) where T : UnityEngine.Object
        {
            _references.Add(reference);
            return await _assetProvider.LoadAsync<T>(reference);
        }

        public async UniTask<T> InstantiateAsync<T>(AssetReference reference, Transform parent = null)
            where T : UnityEngine.Object
        {
            _references.Add(reference);
            return await _assetProvider.InstantiateAsync<T>(reference, parent);
        }
    }
}