using System;
using R3;
using UnityEngine;

namespace TankPlayground.Services
{
    public interface IInputService
    {
        Vector2 PointerPosition { get; }

        Observable<ActionButtonType> ActionPressedObservable { get; }
        event Action<bool> PrimaryFirePressed;
        event Action<Vector2> MovePressed;
        
        void Enable();
        void Disable();
    }
}