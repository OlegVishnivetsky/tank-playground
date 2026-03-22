using Cysharp.Threading.Tasks;

namespace TankPlayground.Services
{
    public interface ISceneLoaderService
    {
        UniTask LoadAsync(SceneName sceneName, bool networked = false);
        void NotifyReady();
        void AddPendingOperation(LoadingOperation operation);
        void AddPreloadOperation(LoadingOperation operation);    
    }
}