using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace TankPlayground.Services
{
    public class AuthService : IAuthService
    {
        public AuthStatus CurrentStatus { get; private set; } = AuthStatus.NotAuthenticated;

        public async UniTask<AuthStatus> AuthenticateAnonymouslyAsync(int maxAttempts = 5)
        {
            try
            {
                if (CurrentStatus == AuthStatus.Authenticating)
                    return CurrentStatus;

                int attempts = 0;
                CurrentStatus = AuthStatus.Authenticating;

                while (CurrentStatus == AuthStatus.Authenticating && attempts < maxAttempts)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        CurrentStatus = AuthStatus.Authenticated;
                        break;
                    }

                    attempts++;
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                }

                CurrentStatus = AuthStatus.Authenticated;
            }
            catch (AuthenticationException e)
            {
                Debug.LogError($"Error while authenticating: {e}");
                CurrentStatus = AuthStatus.Failed;
            }
            
            return CurrentStatus;
        }
    }
}