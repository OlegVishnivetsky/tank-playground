using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TankPlayground.Services
{
    public class InputService : IInputService, IInitializable, Controls.IPlayerActions
    {
        private Controls _controls;

        public Vector2 PointerPosition { get; private set; }
        
        private readonly Subject<ActionButtonType> _actionPressedSubject = new();
        public Observable<ActionButtonType> ActionPressedObservable => _actionPressedSubject;
        
        public event Action<bool> PrimaryFirePressed;
        public event Action<Vector2> MovePressed;
        
        public void Initialize()
        {
            _controls = new();
            _controls.Player.SetCallbacks(this);
            Enable();
        }
        
        public void Enable() => _controls.Player.Enable();
        
        public void Disable() => _controls.Player.Disable();

        public void OnMove(InputAction.CallbackContext context) => 
            MovePressed?.Invoke(context.ReadValue<Vector2>());

        public void OnPrimaryFire(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryFirePressed?.Invoke(true);
            else if (context.canceled)
                PrimaryFirePressed?.Invoke(false);
        }

        public void OnReload(InputAction.CallbackContext context) => _actionPressedSubject.OnNext(ActionButtonType.Reload);

        public void OnAim(InputAction.CallbackContext context) => 
            PointerPosition = context.ReadValue<Vector2>();
    }
}