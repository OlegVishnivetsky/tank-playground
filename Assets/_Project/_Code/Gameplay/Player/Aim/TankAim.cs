using TankPlayground.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TankPlayground.Gameplay
{
    public class TankAim : NetworkBehaviour
    {
        [SerializeField] private Transform _headTransform;
        
        private Camera _camera;
        private IInputService _inputService;
        
        [Inject]
        public void Construct(IInputService inputService) => _inputService = inputService;

        private void Awake() => _camera = Camera.main;

        private void LateUpdate()
        {
            if (!IsOwner)
                return;

            Vector2 pointerPosition = _inputService.PointerPosition;
            Vector2 pointerWorldPosition = Camera.main.ScreenToWorldPoint(pointerPosition);
            _headTransform.up = new()
            {
                x = pointerWorldPosition.x - _headTransform.position.x, 
                y = pointerWorldPosition.y - _headTransform.position.y
            };
        }
    }
}