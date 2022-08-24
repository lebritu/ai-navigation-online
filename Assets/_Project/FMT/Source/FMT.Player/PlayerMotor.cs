using System;
using FMT.Gameplay;
using FMT.Network;
using UnityEngine;
using Mirror;

namespace FMT.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : NetworkBehaviour
    {
        public event Action<MovementCallBack> OnMovementCallBack;
        public event Action<bool> OnMovement;
        public event Action<bool> OnRunning;
        public event Action OnJumping;

        [SerializeField] private Camera _camera;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _cameraPosition;

        [SerializeField] private float _jumpHeight = 1.0f;
        [SerializeField] private float _movementSpeed = 1f;

        private bool _groundedPlayer;
        private bool _isRunning;
        private float _rotationX;
        private float _rotationY;
        private float _sensitiveCam = 5;
        private Vector3 _playerVelocity;
        private InputManager _inputManager;

        private readonly float _gravityValue = -9.81f;
        private readonly float _defaultWalk = 1;
        private readonly float _defaultRunning = 3;

        public override void OnStartAuthority()
        {
            _camera.gameObject.SetActive(true);
            _inputManager = gameObject.AddComponent<InputManager>();
        }

        private void Update()
        {
            if (!hasAuthority) { return; }

            CameraPosition();
            CharacterControllerMovement();
            Running();
        }

        private void CharacterControllerMovement()
        {
            if(_inputManager == null) { return; }

            _groundedPlayer = _characterController.isGrounded;

            float axisX = _inputManager.MovementAxis.x;
            float axisZ = _inputManager.MovementAxis.y;

            Vector3 move = new Vector3(axisX, 0, axisZ);
            move = transform.TransformDirection(move);
            _characterController.Move(move * Time.deltaTime * _movementSpeed);

            if (_inputManager.Jump && _groundedPlayer)
            {
                _playerVelocity.y = 0f;
                _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
                OnJumping?.Invoke();
            }

            _playerVelocity.y += _gravityValue * Time.deltaTime;
            _characterController.Move(_playerVelocity * Time.deltaTime);

            MouseMovement(axisX, axisZ);

            if(_inputManager.MovementAxis.magnitude > 0)
            {
                OnMovement?.Invoke(true);
            }
            else
            {
                OnMovement?.Invoke(false);
            }
        }

        private void MouseMovement(float axisX, float axisZ)
        {
            _rotationY += _inputManager.CameraAxis.x * _sensitiveCam;
            _rotationX -= _inputManager.CameraAxis.y * _sensitiveCam;
            _rotationX = Mathf.Clamp(_rotationX, -70, 80);

            _camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation = Quaternion.Euler(0, _rotationY, 0);

            SendMovementCallBack(axisX, axisZ);
        }

        private void Running()
        {
            if (_inputManager.Running)
            {
                _movementSpeed = _defaultRunning;

                if (!_isRunning) { OnRunning?.Invoke(true); }

                _isRunning = true;
            }
            else if (_isRunning)
            {
                _movementSpeed = _defaultWalk;
                _isRunning = false;
                OnRunning?.Invoke(false);
            }
        }

        private void CameraPosition()
        {
            _camera.transform.position = _cameraPosition.position;

        }

        private void SendMovementCallBack(float axisX, float axisZ)
        {
            MovementCallBack msg = new MovementCallBack
            {
                Spine = _rotationX,
                AxisX = axisX,
                AxisZ = axisZ
            };

            OnMovementCallBack?.Invoke(msg);
        }
    }
}
