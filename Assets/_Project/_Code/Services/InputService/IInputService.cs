using System;
using UnityEngine;

namespace TankPlayground.Services
{
    public interface IInputService
    {
        Vector2 PointerPosition { get; }

        event Action<bool> PrimaryFirePressed;
        event Action<Vector2> MovePressed;
        
        void Enable();
        void Disable();
    }
}