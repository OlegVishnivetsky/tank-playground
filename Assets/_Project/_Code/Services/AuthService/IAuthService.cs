using Cysharp.Threading.Tasks;

namespace TankPlayground.Services
{
    public interface IAuthService
    {
        AuthStatus CurrentStatus { get; }
        
        UniTask<AuthStatus> AuthenticateAnonymouslyAsync(int maxAttempts = 5);
    }
}