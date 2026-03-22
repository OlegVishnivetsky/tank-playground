using System;
using TankPlayground.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TankPlayground.Gameplay
{
    public class TankMovement : NetworkBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Transform _hullTransform;
        
        private TankEntity _entity;
        public IInputService _inputService;
        
        private float _moveSpeed;
        private float _turningRate;
        private Vector2 _moveInput;
        
        [Inject]
        public void Construct(IInputService inputService) => _inputService = inputService;

        private void Awake() => _entity = GetComponent<TankEntity>();

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;
            
            UpdateMovementStats();
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

            UpdateMovementStats();
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

        private void UpdateMovementStats()
        {
            _moveSpeed = _entity.StatsController.GetStatValue(StatType.MoveSpeed);
            _turningRate = _entity.StatsController.GetStatValue(StatType.TurnRate);
        }
    }
}