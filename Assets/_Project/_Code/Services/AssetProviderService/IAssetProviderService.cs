using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace TankPlayground.Services
{
    public interface IAssetProviderService
    {
        UniTask<T> InstantiateAsync<T>(AssetReference reference, Transform parent = null) where T : UnityEngine.Object;
        UniTask<T> LoadAsync<T>(AssetReference reference) where T : UnityEngine.Object;
        void Release(AssetReference reference);
        void Release(GameObject gameObject);
        void ReleaseAll();
    }
}