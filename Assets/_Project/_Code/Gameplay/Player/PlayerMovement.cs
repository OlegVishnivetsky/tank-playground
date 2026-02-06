using TankPlayground.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TankPlayground.Gameplay
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Transform _hullTransform;
        
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _turningRate;
        
        private IInputService _inputService;
        private Vector2 _moveInput;
        
        [Inject]
        public void Construct(IInputService inputService) => _inputService = inputService;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;
            
            _inputService.MovePressed += OnMovePressed;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;
            
            _inputService.MovePressed -= OnMovePressed;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            float zRotation = _moveInput.x * -_turningRate * Time.deltaTime;
            
            _hullTransform.Rotate(new(0f, 0f, zRotation));
        }

        private void FixedUpdate()
        {
            if (!IsOwner)
                return;
            
            _rigidbody2D.velocity = _hullTransform.up * (_moveInput.y * _moveSpeed);
        }

        private void OnMovePressed(Vector2 input) => _moveInput = input;
    }
}